﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Test.Utilities.CSharpSecurityCodeFixVerifier<
    Microsoft.NetFramework.Analyzers.DoNotUseInsecureDtdProcessingAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;
using VerifyVB = Test.Utilities.VisualBasicSecurityCodeFixVerifier<
    Microsoft.NetFramework.Analyzers.DoNotUseInsecureDtdProcessingAnalyzer,
    Microsoft.CodeAnalysis.Testing.EmptyCodeFixProvider>;

namespace Microsoft.NetFramework.Analyzers.UnitTests
{
    public partial class DoNotUseInsecureDtdProcessingAnalyzerTests
    {
        [Fact]
        public async Task UseXmlDocumentSetInnerXmlShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;
using System.Data;

namespace TestNamespace
{
    public class DoNotUseSetInnerXml
    {
        public void TestMethod(string xml)
        {
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(12, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml
Imports System.Data

Namespace TestNamespace
    Public Class DoNotUseSetInnerXml
        Public Sub TestMethod(xml As String)
            Dim doc As New XmlDocument() With { _
                 .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        End Sub
    End Class
End Namespace",
                GetCA3075InnerXmlBasicResultAt(11, 13)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInGetShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;

class TestClass
{
    public XmlDocument Test
    {
        get {
            var xml = """";
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.InnerXml = xml;
            return doc;
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(11, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml

Class TestClass
    Public ReadOnly Property Test() As XmlDocument
        Get
            Dim xml = """"
            Dim doc As New XmlDocument() With { _
                .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
            Return doc
        End Get
    End Property
End Class",
                GetCA3075InnerXmlBasicResultAt(11, 13)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInSetShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;

class TestClass
{
XmlDocument privateDoc;
public XmlDocument GetDoc
        {
            set
            {
                if (value == null)
                {
                    var xml = """";
                    XmlDocument doc = new XmlDocument() { XmlResolver = null };
                    doc.InnerXml = xml;
                    privateDoc = doc;
                }
                else
                    privateDoc = value;
            }
        }
}",
                GetCA3075InnerXmlCSharpResultAt(15, 21)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml

Class TestClass
    Private privateDoc As XmlDocument
    Public WriteOnly Property GetDoc() As XmlDocument
        Set
            If value Is Nothing Then
                Dim xml = """"
                Dim doc As New XmlDocument() With { _
                     .XmlResolver = Nothing _
                }
                doc.InnerXml = xml
                privateDoc = doc
            Else
                privateDoc = value
            End If
        End Set
    End Property
End Class",
                GetCA3075InnerXmlBasicResultAt(13, 17)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInTryBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

class TestClass
{
    private void TestMethod()
    {
        try
        {
            var xml = """";
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        }
        catch (Exception) { throw; }
        finally { }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(13, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Class TestClass
    Private Sub TestMethod()
        Try
            Dim xml = """"
            Dim doc As New XmlDocument() With { _
                 .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        Catch generatedExceptionName As Exception
            Throw
        Finally
        End Try
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(12, 13)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInCatchBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

class TestClass
{
    private void TestMethod()
    {
        try { }
        catch (Exception)
        {
            var xml = """";
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        }
        finally { }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(14, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Dim xml = """"
            Dim doc As New XmlDocument() With { _
                 .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        Finally
        End Try
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(13, 13)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInFinallyBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

class TestClass
{
    private void TestMethod()
    {
        try { }
        catch (Exception) { throw; }
        finally
        {
            var xml = """";
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(15, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Throw
        Finally
            Dim xml = """"
            Dim doc As New XmlDocument() With { _
                 .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        End Try
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(15, 13)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInAsyncAwaitShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Threading.Tasks;
using System.Xml;

class TestClass
{
    private async Task TestMethod()
    {
        await Task.Run(() => {
            var xml = """";
            XmlDocument doc = new XmlDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        });
    }

    private async void TestMethod2()
    {
        await TestMethod();
    }
}",
                GetCA3075InnerXmlCSharpResultAt(12, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Threading.Tasks
Imports System.Xml

Class TestClass
    Private Async Function TestMethod() As Task
        Await Task.Run(Function() 
        Dim xml = """"
        Dim doc As New XmlDocument() With { _
            .XmlResolver = Nothing _
        }
        doc.InnerXml = xml

End Function)
    End Function

    Private Async Sub TestMethod2()
        Await TestMethod()
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(12, 9)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInDelegateShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;

class TestClass
{
    delegate void Del();

    Del d = delegate () {
        var xml = """";
        XmlDocument doc = new XmlDocument() { XmlResolver = null };
        doc.InnerXml = xml;
    };
}",
                GetCA3075InnerXmlCSharpResultAt(11, 9)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml

Class TestClass
    Private Delegate Sub Del()

    Private d As Del = Sub() 
    Dim xml = """"
    Dim doc As New XmlDocument() With { _
        .XmlResolver = Nothing _
    }
    doc.InnerXml = xml

End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(12, 5)
            );
        }

        [Fact]
        public async Task UseXmlDocumentSetInnerXmlInlineShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;
using System.Data;

namespace TestNamespace
{
    public class DoNotUseSetInnerXml
    {
        public void TestMethod(string xml)
        {
            XmlDocument doc = new XmlDocument()
            {
                XmlResolver = null,
                InnerXml = xml
            };
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(14, 17)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml
Imports System.Data

Namespace TestNamespace
    Public Class DoNotUseSetInnerXml
        Public Sub TestMethod(xml As String)
            Dim doc As New XmlDocument() With { _
                .XmlResolver = Nothing, _
                .InnerXml = xml _
            }
        End Sub
    End Class
End Namespace",
                GetCA3075InnerXmlBasicResultAt(10, 17)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentInnerXmlShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;
using System.Data;

namespace TestNamespace
{
    public class DoNotUseSetInnerXml
    {
        public void TestMethod(string xml)
        {
            XmlDataDocument doc = new XmlDataDocument(){ XmlResolver = null };
            doc.InnerXml = xml;
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(12, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml
Imports System.Data

Namespace TestNamespace
    Public Class DoNotUseSetInnerXml
        Public Sub TestMethod(xml As String)
            Dim doc As New XmlDataDocument() With { _
                .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        End Sub
    End Class
End Namespace",
                GetCA3075InnerXmlBasicResultAt(11, 13)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInGetShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;

class TestClass
{
    public XmlDataDocument Test
    {
        get {
            var xml = """";
            XmlDataDocument doc = new XmlDataDocument() { XmlResolver = null };
            doc.InnerXml = xml;
            return doc;
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(11, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml

Class TestClass
    Public ReadOnly Property Test() As XmlDataDocument
        Get
            Dim xml = """"
            Dim doc As New XmlDataDocument() With { _
                .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
            Return doc
        End Get
    End Property
End Class",
                GetCA3075InnerXmlBasicResultAt(11, 13)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInSetShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;

class TestClass
{
XmlDataDocument privateDoc;
public XmlDataDocument GetDoc
        {
            set
            {
                if (value == null)
                {
                    var xml = """";
                    XmlDataDocument doc = new XmlDataDocument() { XmlResolver = null };
                    doc.InnerXml = xml;
                    privateDoc = doc;
                }
                else
                    privateDoc = value;
            }
        }
}",
                GetCA3075InnerXmlCSharpResultAt(15, 21)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml

Class TestClass
    Private privateDoc As XmlDataDocument
    Public WriteOnly Property GetDoc() As XmlDataDocument
        Set
            If value Is Nothing Then
                Dim xml = """"
                Dim doc As New XmlDataDocument() With { _
                    .XmlResolver = Nothing _
                }
                doc.InnerXml = xml
                privateDoc = doc
            Else
                privateDoc = value
            End If
        End Set
    End Property
End Class
",
                GetCA3075InnerXmlBasicResultAt(13, 17)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInTryBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

class TestClass
{
    private void TestMethod()
    {
        try
        {
            var xml = """";
            XmlDataDocument doc = new XmlDataDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        }
        catch (Exception) { throw; }
        finally { }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(13, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Class TestClass
    Private Sub TestMethod()
        Try
            Dim xml = """"
            Dim doc As New XmlDataDocument() With { _
                 .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        Catch generatedExceptionName As Exception
            Throw
        Finally
        End Try
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(12, 13)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInCatchBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

class TestClass
{
    private void TestMethod()
    {
        try { }
        catch (Exception)
        {
            var xml = """";
            XmlDataDocument doc = new XmlDataDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        }
        finally { }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(14, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Dim xml = """"
            Dim doc As New XmlDataDocument() With { _
                .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        Finally
        End Try
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(13, 13)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInFinallyBlockShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System;
using System.Xml;

class TestClass
{
    private void TestMethod()
    {
        try { }
        catch (Exception) { throw; }
        finally
        {
            var xml = """";
            XmlDataDocument doc = new XmlDataDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(15, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System
Imports System.Xml

Class TestClass
    Private Sub TestMethod()
        Try
        Catch generatedExceptionName As Exception
            Throw
        Finally
            Dim xml = """"
            Dim doc As New XmlDataDocument() With { _
                .XmlResolver = Nothing _
            }
            doc.InnerXml = xml
        End Try
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(15, 13)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInAsyncAwaitShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Threading.Tasks;
using System.Xml;

class TestClass
{
    private async Task TestMethod()
    {
        await Task.Run(() => {
            var xml = """";
            XmlDataDocument doc = new XmlDataDocument() { XmlResolver = null };
            doc.InnerXml = xml;
        });
    }

    private async void TestMethod2()
    {
        await TestMethod();
    }
}",
                GetCA3075InnerXmlCSharpResultAt(12, 13)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Threading.Tasks
Imports System.Xml

Class TestClass
    Private Async Function TestMethod() As Task
        Await Task.Run(Function() 
        Dim xml = """"
        Dim doc As New XmlDataDocument() With { _
            .XmlResolver = Nothing _
        }
        doc.InnerXml = xml

End Function)
    End Function

    Private Async Sub TestMethod2()
        Await TestMethod()
    End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(12, 9)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInDelegateShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;

class TestClass
{
    delegate void Del();

    Del d = delegate () {
        var xml = """";
        XmlDataDocument doc = new XmlDataDocument() { XmlResolver = null };
        doc.InnerXml = xml;
    };
}",
                GetCA3075InnerXmlCSharpResultAt(11, 9)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml

Class TestClass
    Private Delegate Sub Del()

    Private d As Del = Sub() 
    Dim xml = """"
    Dim doc As New XmlDataDocument() With { _
        .XmlResolver = Nothing _
    }
    doc.InnerXml = xml

End Sub
End Class",
                GetCA3075InnerXmlBasicResultAt(12, 5)
            );
        }

        [Fact]
        public async Task UseXmlDataDocumentSetInnerXmlInlineShouldGenerateDiagnostic()
        {
            await VerifyCS.VerifyAnalyzerAsync(@"
using System.Xml;
using System.Data;

namespace TestNamespace
{
    public class DoNotUseSetInnerXml
    {
        public void TestMethod(string xml)
        {
            XmlDataDocument doc = new XmlDataDocument()
            {
                XmlResolver = null,
                InnerXml = xml
            };
        }
    }
}",
                GetCA3075InnerXmlCSharpResultAt(14, 17)
            );

            await VerifyVB.VerifyAnalyzerAsync(@"
Imports System.Xml
Imports System.Data

Namespace TestNamespace
    Public Class DoNotUseSetInnerXml
        Public Sub TestMethod(xml As String)
            Dim doc As New XmlDataDocument() With { _
                .XmlResolver = Nothing, _
                .InnerXml = xml _
            }
        End Sub
    End Class
End Namespace",
                GetCA3075InnerXmlBasicResultAt(10, 17)
            );
        }

        private DiagnosticResult GetCA3075InnerXmlCSharpResultAt(int line, int column)
        {
            return new DiagnosticResult(DoNotUseInsecureDtdProcessingAnalyzer.RuleDoNotUseInsecureDtdProcessing).WithLocation(line, column).WithArguments(MicrosoftNetFrameworkAnalyzersResources.DoNotUseSetInnerXmlMessage);
        }

        private DiagnosticResult GetCA3075InnerXmlBasicResultAt(int line, int column)
        {
            return new DiagnosticResult(DoNotUseInsecureDtdProcessingAnalyzer.RuleDoNotUseInsecureDtdProcessing).WithLocation(line, column).WithArguments(MicrosoftNetFrameworkAnalyzersResources.DoNotUseSetInnerXmlMessage);
        }
    }
}
