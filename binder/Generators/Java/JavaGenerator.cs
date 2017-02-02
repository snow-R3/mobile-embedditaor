using System;
using System.Collections.Generic;
using CppSharp;
using CppSharp.AST;
using CppSharp.Generators;
using Template = CppSharp.Generators.Template;

namespace MonoEmbeddinator4000.Generators
{
    public class JavaGenerator : Generator
    {
        public JavaGenerator(BindingContext context) : base(context)
        {
        }

        public override List<Template> Generate(TranslationUnit unit)
        {
            var sources = new JavaSources(Context, unit);

            return new List<Template> { sources };
        }

        public static string GenId(string id)
        {
            return "__" + id;
        }

        public static string AssemblyId(TranslationUnit unit)
        {
            return GenId(unit.FileName).Replace('.', '_');
        }

        public static JavaTypePrinter GetJavaTypePrinter()
        {
            var typePrinter = new JavaTypePrinter
            {
                PrintScopeKind = CppTypePrintScopeKind.Qualified,
                PrintVariableArrayAsPointers = true
            };

            return typePrinter;
        }
    }

    public abstract class JavaTemplate : Template, IDeclVisitor<bool>
    {
        public TranslationUnit Unit;

        public JavaTemplate(BindingContext context, TranslationUnit unit)
            : base(context, unit)
        {
            Unit = unit;
        }

        public string GeneratedIdentifier(string id)
        {
            return CGenerator.GenId(id);
        }

        public string QualifiedName(Declaration decl)
        {
            if (Options.GeneratorKind == GeneratorKind.CPlusPlus)
                return decl.Name;

            return decl.QualifiedName;
        }

        public CManagedToNativeTypePrinter CTypePrinter
        {
            get
            {
                return CGenerator.GetCTypePrinter(Options.GeneratorKind);
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

            Write("{0}{1} {2}_{3}(", isSource ? string.Empty : "MONO_M2N_API ",
                retType, @class.QualifiedName, method.Name);

            Write(CTypePrinter.VisitParameters(method.Parameters));

            Write(")");
        }

        public virtual string GenerateClassObjectAlloc(string type)
        {
            return $"({type}*) calloc(1, sizeof({type}))";
        }

        public virtual bool VisitTypedefDecl(TypedefDecl typedef)
        {
            PushBlock();

            var typeName = typedef.Type.Visit(CTypePrinter);
            WriteLine("typedef {0} {1};", typeName, typedef.Name);

            var newlineKind = NewLineKind.BeforeNextBlock;

            var declarations = typedef.Namespace.Declarations;
            var newIndex = declarations.FindIndex(d => d == typedef) + 1;
            if (newIndex < declarations.Count)
            {
                if (declarations[newIndex] is TypedefDecl)
                    newlineKind = NewLineKind.Never;
            }

            PopBlock(newlineKind);

            return true;
        }

        public virtual bool VisitNamespace (Namespace @namespace)
        {
            return VisitDeclContext(@namespace);
        }

        public virtual bool VisitFieldDecl(Field field)
        {
            WriteLine("{0} {1};", field.Type, field.Name);
            return true;
        }

        public virtual bool VisitProperty(Property property)
        {
            return true;
        }

        #region IDeclVisitor methods

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
