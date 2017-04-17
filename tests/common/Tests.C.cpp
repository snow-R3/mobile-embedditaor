﻿#define CATCH_CONFIG_MAIN
#include <catch.hpp>

#include "managed.h"
#include "glib.h"

#include "float.h"

TEST_CASE("Types.C", "[C][Types]") {
    BuiltinTypes* bt = BuiltinTypes_new();
    BuiltinTypes_ReturnsVoid(bt);
    REQUIRE(BuiltinTypes_ReturnsBool(bt)   == true);
    REQUIRE(BuiltinTypes_ReturnsSByte(bt)  == -5);
    REQUIRE(BuiltinTypes_ReturnsByte(bt)   ==  5);
    REQUIRE(BuiltinTypes_ReturnsShort(bt)  == -5);
    REQUIRE(BuiltinTypes_ReturnsUShort(bt) ==  5);
    REQUIRE(BuiltinTypes_ReturnsInt(bt)    == -5);
    REQUIRE(BuiltinTypes_ReturnsUInt(bt)   ==  5);
    REQUIRE(BuiltinTypes_ReturnsLong(bt)   == -5);
    REQUIRE(BuiltinTypes_ReturnsULong(bt)  ==  5);
    REQUIRE(BuiltinTypes_ReturnsChar(bt) == 'a');
    REQUIRE(strcmp(BuiltinTypes_ReturnsString(bt), "Mono") == 0);

    REQUIRE(BuiltinTypes_PassAndReturnsBool(bt, true) == true);
    REQUIRE(BuiltinTypes_PassAndReturnsSByte(bt, -5) == -5);
    REQUIRE(BuiltinTypes_PassAndReturnsByte(bt, 5) == 5);
    REQUIRE(BuiltinTypes_PassAndReturnsShort(bt, -5) == -5);
    REQUIRE(BuiltinTypes_PassAndReturnsUShort(bt, 5) == 5);
    REQUIRE(BuiltinTypes_PassAndReturnsInt(bt, -5) == -5);
    REQUIRE(BuiltinTypes_PassAndReturnsUInt(bt, 5) == 5);
    REQUIRE(BuiltinTypes_PassAndReturnsLong(bt, -5) == -5);
    REQUIRE(BuiltinTypes_PassAndReturnsULong(bt, 5) == 5);
    REQUIRE(BuiltinTypes_PassAndReturnsChar(bt, 'a') == 'a');
    REQUIRE(strcmp(BuiltinTypes_PassAndReturnsString(bt, "Mono"), "Mono") == 0);

    int OutInt = 0;
    BuiltinTypes_PassOutInt(bt, &OutInt);
    REQUIRE(OutInt == 5);

    int RefInt = 0;
    BuiltinTypes_PassRefInt(bt, &RefInt);
    REQUIRE(RefInt == 10);

    GString* RefOut = g_string_new("");
    BuiltinTypes_PassOutString(bt, RefOut);
    REQUIRE(strcmp(RefOut->str, "Mono") == 0);

    GString* RefStr = g_string_new("monomono");
    BuiltinTypes_PassRefString(bt, RefStr);
    REQUIRE(strcmp(RefStr->str, "Mono") == 0);
}

TEST_CASE("Properties", "[C][Properties]") {
    REQUIRE(Platform_get_IsWindows() == false);

    Platform_set_ExitCode(255);
    REQUIRE(Platform_get_ExitCode() == 255);

    REQUIRE(Properties_Query_get_UniversalAnswer() == 42);

    Properties_Query* prop = Properties_Query_new();
    REQUIRE(Properties_Query_get_IsGood(prop) == true);
    REQUIRE(Properties_Query_get_IsBad(prop) == false);
    REQUIRE(Properties_Query_get_Answer(prop) == 42);
    Properties_Query_set_Answer(prop, 911);
    REQUIRE(Properties_Query_get_Answer(prop) == 911);

    REQUIRE(Properties_Query_get_IsSecret(prop) == false);
    Properties_Query_set_Secret(prop, 1);
    REQUIRE(Properties_Query_get_IsSecret(prop) == true);
}

TEST_CASE("Namespaces.C", "[C][Namespaces]") {
    ClassWithoutNamespace* nonamespace = ClassWithoutNamespace_new();
    REQUIRE(strcmp(ClassWithoutNamespace_ToString(nonamespace), "ClassWithoutNamespace") == 0);

    First_ClassWithSingleNamespace* singlenamespace = First_ClassWithSingleNamespace_new();
    REQUIRE(strcmp(First_ClassWithSingleNamespace_ToString(singlenamespace), "First.ClassWithSingleNamespace") == 0);

    First_Second_ClassWithNestedNamespace* nestednamespaces = First_Second_ClassWithNestedNamespace_new();
    REQUIRE(strcmp(First_Second_ClassWithNestedNamespace_ToString(nestednamespaces), "First.Second.ClassWithNestedNamespace") == 0);

    First_Second_Third_ClassWithNestedNamespace* nestednamespaces2 = First_Second_Third_ClassWithNestedNamespace_new();
    REQUIRE(strcmp(First_Second_Third_ClassWithNestedNamespace_ToString(nestednamespaces2), "First.Second.Third.ClassWithNestedNamespace") == 0);
}

TEST_CASE("Constructors.C", "[C][Constructors]") {
    Constructors_Unique* unique = Constructors_Unique_new_1();
    REQUIRE(Constructors_Unique_get_Id(unique) == 1);

    Constructors_Unique* unique_init_id = Constructors_Unique_new_2(911);
    REQUIRE(Constructors_Unique_get_Id(unique_init_id) == 911);

    Constructors_SuperUnique* super_unique_default_init = Constructors_SuperUnique_new();
    REQUIRE(Constructors_Unique_get_Id(super_unique_default_init) == 411);

    Constructors_Implicit* implicit = Constructors_Implicit_new();
    REQUIRE(strcmp(Constructors_Implicit_get_TestResult(implicit), "OK") == 0);

    Constructors_AllTypeCode* all1 = Constructors_AllTypeCode_new(true, USHRT_MAX, "Mono");
    REQUIRE(Constructors_AllTypeCode_get_TestResult(all1) == true);

    Constructors_AllTypeCode* all2 = Constructors_AllTypeCode_new_1(SCHAR_MAX, SHRT_MAX, INT_MAX, LONG_MAX);
    REQUIRE(Constructors_AllTypeCode_get_TestResult(all2) == true);

    Constructors_AllTypeCode* all3 = Constructors_AllTypeCode_new_2(UCHAR_MAX, USHRT_MAX, UINT_MAX, ULONG_MAX);
    REQUIRE(Constructors_AllTypeCode_get_TestResult(all3) == true);

    Constructors_AllTypeCode* all4 = Constructors_AllTypeCode_new_3(FLT_MAX, DBL_MAX);
    REQUIRE(Constructors_AllTypeCode_get_TestResult(all4) == true);
}

TEST_CASE("Enums.C", "[C][Enums]") {
    REQUIRE(Enums_EnumTypes_PassEnum(Enum_Two) == 2);
    REQUIRE(Enums_EnumTypes_PassEnum(Enum_Three) == 3);

    REQUIRE(Enums_EnumTypes_PassEnumByte(EnumByte_Two) == 2);
    REQUIRE(Enums_EnumTypes_PassEnumByte(EnumByte_Three) == 3);

    REQUIRE(Enums_EnumTypes_PassEnumFlags(EnumFlags_FlagOne) == (1 << 0));
    REQUIRE(Enums_EnumTypes_PassEnumFlags(EnumFlags_FlagTwo) == (1 << 2));
}

TEST_CASE("Arrays.C", "[C][Arrays]") {
    char _byte_arr[] = { 1, 2, 3 };
    _UnsignedcharArray _byte;
    _byte.array = g_array_sized_new(/*zero_terminated=*/false,
        /*clear=*/true, sizeof(char), G_N_ELEMENTS(_byte_arr));
    g_array_append_vals (_byte.array, _byte_arr, G_N_ELEMENTS(_byte_arr));

    int _sum = Arrays_ArrayTypes_SumByteArray(_byte);
    REQUIRE(_sum == 6);

    _IntArray _int = Arrays_ArrayTypes_ReturnsIntArray();
    REQUIRE(_int.array->len == 3);
    REQUIRE(g_array_index(_int.array, int, 0) == 1);
    REQUIRE(g_array_index(_int.array, int, 1) == 2);
    REQUIRE(g_array_index(_int.array, int, 2) == 3);

    _CharArray _string = Arrays_ArrayTypes_ReturnsStringArray();
    REQUIRE(_string.array->len == 3);
    REQUIRE(strcmp(g_array_index(_string.array, gchar*, 0), "1") == 0);
    REQUIRE(strcmp(g_array_index(_string.array, gchar*, 1), "2") == 0);
    REQUIRE(strcmp(g_array_index(_string.array, gchar*, 2), "3") == 0);
}
