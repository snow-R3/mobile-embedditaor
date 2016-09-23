﻿using System;
using System.Collections.Generic;
using CppSharp.AST;
using CppSharp.Generators;

namespace MonoManagedToNative.Generators
{
    public class CGenerator : Generator
    {
        internal Options Options { get; set; }

        public CGenerator(BindingContext context, Options options) : base(context)
        {
            Options = options; 
        }

        public override List<Template> Generate(TranslationUnit unit)
        {
            var headers = new CHeaders(Context, Options, unit);
            var sources = new CSources(Context, Options, unit);

            return new List<Template> { headers, sources };
        }

        public static string GenId(string id)
        {
            return "__" + id;
        }

        private static CppTypePrintFlavorKind GetTypePrinterFlavorKind(GeneratorKind kind)
        {
            switch (kind)
            {
                case GeneratorKind.C:
                    return CppTypePrintFlavorKind.C;
                case GeneratorKind.CPlusPlus:
                    return CppTypePrintFlavorKind.Cpp;
                case GeneratorKind.ObjectiveC:
                    return CppTypePrintFlavorKind.ObjC;
            }

            throw new NotImplementedException();
        }

        public static CManagedToNativeTypePrinter GetCTypePrinter(GeneratorKind kind)
        {
            var typePrinter = new CManagedToNativeTypePrinter
            {
                PrintScopeKind = CppTypePrintScopeKind.Qualified,
                PrintFlavorKind = GetTypePrinterFlavorKind(kind),
                PrintVariableArrayAsPointers = true
            };

            return typePrinter;
        }
    }

    public abstract class CTemplate : Template, IDeclVisitor<bool>
    {
        public TranslationUnit Unit;

        public CTemplate(BindingContext context, Options options,
            TranslationUnit unit) : base(context, options)
        {
            Declaration.QualifiedNameSeparator = "_";
            Unit = unit;
        }

        public override string Name
        {
            get { return Unit.FileName; }
        }

        public string GeneratedIdentifier(string id)
        {
            return CGenerator.GenId(id);
        }

        public string QualifiedName(Declaration decl)
        {
            if (Options.Language == GeneratorKind.CPlusPlus)
                return decl.Name;

            return decl.QualifiedName;
        }

        public CManagedToNativeTypePrinter CTypePrinter
        {
            get
            {
                return CGenerator.GetCTypePrinter(Options.Language);
            }
        }

        public bool VisitDeclContext(DeclarationContext ctx)
        {
            foreach (var decl in ctx.Declarations)
                if (!decl.Ignore)
                    decl.Visit(this);

            return true;
        }

        public void WriteInclude(string include)
        {
            if (Options.GenerateSupportFiles)
                WriteLine("#include \"{0}\"", include);
            else
                WriteLine("#include <{0}>", include);
        }

        public void GenerateFilePreamble()
        {
            WriteLine("/*");
            WriteLine(" * This is autogenerated code.");
            WriteLine(" * Do not edit this file or all your changes will be lost after re-generation.");
            WriteLine(" */");
        }

        public virtual void GenerateMethodSignature(Method method, bool isSource = true)
        {
            var @class = method.Namespace as Class;
            var retType = method.ReturnType.Visit(CTypePrinter);

            Write("{0}{1} {2}_{3}(", isSource ? string.Empty : string.Empty, // "MONO_M2N_API ",
                retType, @class.QualifiedName, method.Name);

            Write(CTypePrinter.VisitParameters(method.Parameters));

            Write(")");
        }

        #region IDeclVisitor methods

        public virtual bool VisitNamespace (Namespace @namespace)
        {
            return VisitDeclContext(@namespace);
        }

        public virtual bool VisitClassDecl(Class @class)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitClassTemplateDecl(ClassTemplate template)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitClassTemplateSpecializationDecl(ClassTemplateSpecialization specialization)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitDeclaration(Declaration decl)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitEnumDecl(Enumeration @enum)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitEnumItemDecl(Enumeration.Item item)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitEvent(Event @event)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFieldDecl(Field field)
        {
            return true;
        }

        public virtual bool VisitFriend(Friend friend)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFunctionDecl(Function function)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFunctionTemplateDecl(FunctionTemplate template)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitFunctionTemplateSpecializationDecl(FunctionTemplateSpecialization spec)
        {
            throw new NotImplementedException();
        }        

        public virtual bool VisitMacroDefinition(MacroDefinition macro)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitMethodDecl(Method method)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitNonTypeTemplateParameterDecl(NonTypeTemplateParameter nonTypeTemplateParameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitParameterDecl(Parameter parameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitProperty(Property property)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTemplateParameterDecl(TypeTemplateParameter templateParameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTemplateTemplateParameterDecl(TemplateTemplateParameter templateTemplateParameter)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTypeAliasDecl(TypeAlias typeAlias)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTypeAliasTemplateDecl(TypeAliasTemplate typeAliasTemplate)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitTypedefDecl(TypedefDecl typedef)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitVariableDecl(Variable variable)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitVarTemplateDecl(VarTemplate template)
        {
            throw new NotImplementedException();
        }

        public virtual bool VisitVarTemplateSpecializationDecl(VarTemplateSpecialization spec)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
