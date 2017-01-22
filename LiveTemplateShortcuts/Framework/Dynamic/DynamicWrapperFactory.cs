using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace LiveTemplateShortcuts.Framework.Dynamic
{
    public class DynamicWrapperFactory
    {
        private static readonly Dictionary<Type, Type> GeneratedTypeCache = new Dictionary<Type, Type>();

        public Type CreateDynamicWrapper(Type wrapType)
        {
            if (GeneratedTypeCache.TryGetValue(wrapType, out var type))
            {
                return type;
            }

            const AssemblyBuilderAccess access = AssemblyBuilderAccess.RunAndSave;
            const TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit;

            var currentDomain = AppDomain.CurrentDomain;
            string name1 = $"{wrapType.Name}";
            string assemblyName = $"{name1}ProxyAssembly";
            string name2 = $"{name1}Module";
            var name3 = new AssemblyName(assemblyName);

            var assemblyBuilder = currentDomain.DefineDynamicAssembly(name3, access);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(name2, $"{name2}.mod", true);

            var typeBuilder = moduleBuilder.DefineType(name1, typeAttributes);
            var fields = wrapType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(fieldInfo => new FieldSetterDeclaration
                {
                    Type = fieldInfo.FieldType,
                    FieldInfo = fieldInfo
                }).Concat(wrapType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Select(propInfo => new FieldSetterDeclaration
                {
                    Type = propInfo.PropertyType,
                    PropertyInfo = propInfo
                }));

            // TODO: May be able to pick a bunch of interfaces instead.. And then collate the elements of these.
            fields = fields.Where(x => x.Type.IsVisible);

            CreateClassConstructor(wrapType, typeBuilder, fields);
            var resultType = typeBuilder.CreateType();
            GeneratedTypeCache.Add(wrapType, resultType);

            assemblyBuilder.Save($"{assemblyName}.dll");
            return resultType;
        }

        private static void CreateClassConstructor(Type wrappedType, TypeBuilder typeBuilder, IEnumerable<FieldSetterDeclaration> fieldSetterDeclarations)
        {
            const MethodAttributes ctorAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
            var constructorBuilder = typeBuilder.DefineConstructor(ctorAttributes, CallingConventions.Standard, new[] { wrappedType });
            constructorBuilder.SetImplementationFlags(MethodImplAttributes.IL);

            var objectCtor = typeof(object).GetConstructor(Type.EmptyTypes);
            Debug.Assert(objectCtor != null, "objectCtor != null");

            var wrappedObject = typeBuilder.DefineField("__wrappedObject", wrappedType, FieldAttributes.Private);

            var ctoril = constructorBuilder.GetILGenerator();
            ctoril.Emit(OpCodes.Ldarg_0);
            ctoril.Emit(OpCodes.Call, objectCtor);
            ctoril.Emit(OpCodes.Nop);
            ctoril.Emit(OpCodes.Nop);

            ctoril.Emit(OpCodes.Ldarg_0);
            ctoril.Emit(OpCodes.Ldarg_1);
            ctoril.Emit(OpCodes.Stfld, wrappedObject);
            ctoril.Emit(OpCodes.Nop);

            // Initialize fields from wrapped object
            foreach (var decl in fieldSetterDeclarations)
            {
                if (decl.FieldInfo != null &&
                    decl.FieldInfo.IsPrivate == false)
                {
                    var field = typeBuilder.DefineField(decl.FieldInfo.Name, decl.FieldInfo.FieldType, FieldAttributes.Public);

                    // May need to handle FieldAccessException
                    ctoril.Emit(OpCodes.Ldarg_0);
                    ctoril.Emit(OpCodes.Ldarg_1);
                    ctoril.Emit(OpCodes.Ldfld, decl.FieldInfo);
                    ctoril.Emit(OpCodes.Stfld, field);
                }
                else if (decl.FieldInfo != null &&
                         decl.FieldInfo.IsPrivate)
                {
                    // TODO DSS: Attempt, but not working very well - BadImageFormat exception
                    // and I can't find the cause.

                    // Create a property to store the reflection into calling the private field
                    //const MethodAttributes propertyGetterAttributes = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName;
                    //var prop = typeBuilder.DefineProperty(decl.FieldInfo.Name, PropertyAttributes.None, decl.Type, Type.EmptyTypes);
                    //var getterBuilder = typeBuilder.DefineMethod($"get_{decl.FieldInfo.Name}", propertyGetterAttributes, decl.Type, Type.EmptyTypes);
                    //prop.SetGetMethod(getterBuilder);

                    //var getteril = getterBuilder.GetILGenerator();
                    //getteril.Emit(OpCodes.Ldtoken, decl.Type);
                    //getteril.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));

                    //getteril.Emit(OpCodes.Ldstr, decl.FieldInfo.Name);
                    //getteril.Emit(OpCodes.Ldc_I4_S, (byte) 36); // BindingFlags.Instance | BindingFlags.NonPublic
                    //getteril.Emit(OpCodes.Call, typeof(Type).GetMethod("GetField", new[] { typeof(string), typeof(BindingFlags) }));

                    //getteril.Emit(OpCodes.Ldarg_0);
                    //getteril.Emit(OpCodes.Ldfld, wrappedObject);
                    //getteril.Emit(OpCodes.Callvirt, typeof(FieldInfo).GetMethod("GetValue"));
                    //getteril.Emit(OpCodes.Castclass, decl.Type);
                    //getteril.Emit(OpCodes.Ret);
                }

                else if (decl.PropertyInfo.GetGetMethod(false) != null &&
                         decl.PropertyInfo.GetGetMethod(false).IsPrivate == false)
                {
                    var propertyInfo = decl.PropertyInfo;
                    var field = typeBuilder.DefineField(propertyInfo.Name, propertyInfo.PropertyType, FieldAttributes.Public);

                    ctoril.Emit(OpCodes.Ldarg_0);
                    ctoril.Emit(OpCodes.Ldarg_1);
                    ctoril.Emit(OpCodes.Call, decl.PropertyInfo.GetGetMethod(false));
                    ctoril.Emit(OpCodes.Stfld, field);
                }
                else if (decl.PropertyInfo.GetGetMethod(true) != null &&
                         decl.PropertyInfo.GetGetMethod(true).IsPrivate)
                {
                    //typeof(Type).GetMethod("GetProperty", new[] { typeof(string) });
                }
            }

            ctoril.Emit(OpCodes.Ret);
        }

        private class FieldSetterDeclaration
        {
            public Type Type { get; set; }
            public FieldInfo FieldInfo { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
        }
    }
}