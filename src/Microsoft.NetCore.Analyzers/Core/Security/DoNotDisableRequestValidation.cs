﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Linq;
using Analyzer.Utilities;
using Analyzer.Utilities.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace Microsoft.NetCore.Analyzers.Security
{
    [DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
    public sealed class DoNotDisableRequestValidation : DiagnosticAnalyzer
    {
        internal const string DiagnosticId = "CA5363";
        private static readonly LocalizableString s_Title = new LocalizableResourceString(
            nameof(SystemSecurityCryptographyResources.DoNotDisableRequestValidation),
            SystemSecurityCryptographyResources.ResourceManager,
            typeof(SystemSecurityCryptographyResources));
        private static readonly LocalizableString s_Message = new LocalizableResourceString(
            nameof(SystemSecurityCryptographyResources.DoNotDisableRequestValidationMessage),
            SystemSecurityCryptographyResources.ResourceManager,
            typeof(SystemSecurityCryptographyResources));
        private static readonly LocalizableString s_Description = new LocalizableResourceString(
            nameof(SystemSecurityCryptographyResources.DoNotDisableRequestValidationDescription),
            SystemSecurityCryptographyResources.ResourceManager,
            typeof(SystemSecurityCryptographyResources));

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(
                DiagnosticId,
                s_Title,
                s_Message,
                DiagnosticCategory.Security,
                DiagnosticHelpers.DefaultDiagnosticSeverity,
                isEnabledByDefault: DiagnosticHelpers.EnabledByDefaultIfNotBuildingVSIX,
                description: s_Description,
                helpLinkUri: null,
                customTags: WellKnownDiagnosticTags.Telemetry);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            // Security analyzer - analyze and report diagnostics on generated code.
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterCompilationStartAction(
                (CompilationStartAnalysisContext compilationStartAnalysisContext) =>
                {
                    var compilation = compilationStartAnalysisContext.Compilation;
                    var wellKnownTypeProvider = WellKnownTypeProvider.GetOrCreate(compilationStartAnalysisContext.Compilation);

                    if (!wellKnownTypeProvider.TryGetTypeByMetadataName(
                                WellKnownTypeNames.SystemWebMvcValidateInputAttribute,
                                out INamedTypeSymbol validateInputAttributeTypeSymbol))
                    {
                        return;
                    }

                    compilationStartAnalysisContext.RegisterSymbolAction(
                        (SymbolAnalysisContext symbolAnalysisContext) =>
                        {
                            var symbol = symbolAnalysisContext.Symbol;
                            var typeSymbol = symbol.ContainingType;

                            if (typeSymbol == null)
                            {
                                return;
                            }

                            var attr = symbol.GetAttributes().FirstOrDefault(s => s.AttributeClass.Equals(validateInputAttributeTypeSymbol));

                            // If the method doesn't have the ValidateInput attribute, check its type.
                            if (attr == null)
                            {
                                symbol = typeSymbol;
                                attr = symbol.GetAttributes().FirstOrDefault(s => s.AttributeClass.Equals(validateInputAttributeTypeSymbol));
                            }

                            // By default, request validation is enabled.
                            if (attr == null)
                            {
                                return;
                            }

                            var constructorArguments = attr.ConstructorArguments;

                            if (constructorArguments.Length == 1 &&
                                constructorArguments[0].Kind == TypedConstantKind.Primitive &&
                                constructorArguments[0].Value.Equals(false))
                            {
                                symbolAnalysisContext.ReportDiagnostic(
                                    symbol.CreateDiagnostic(
                                        Rule,
                                        symbol.Name));
                            }
                        }, SymbolKind.Method);
                });
        }
    }
}
