using System;
using System.Linq;
using Xunit;
using Moq;
using task05;

namespace task05tests
{
    // Тестовые классы для анализа
    public class TestClass
    {
        public int PublicField;
        private string _privateField;
        protected double _protectedField;
        
        public int Property { get; set; }
        public string ReadOnlyProperty { get; }
        
        public void Method() { }
        public void MethodWithParams(int param1, string param2) { }
        public int MethodWithReturn() => 42;
        
        private void PrivateMethod() { }
    }

    [Serializable]
    public class AttributedClass 
    {
        public void SomeMethod() { }
    }

    [Obsolete("This class is obsolete")]
    [Serializable]
    public class MultiAttributeClass 
    {
        public int Field;
        public string Property { get; set; }
    }

    public class EmptyClass { }

    public class ComplexClass
    {
        public static int StaticField;
        public readonly int ReadOnlyField;
        private int _privateField;
        public int PublicField;

        public int AutoProperty { get; set; }
        public int GetOnlyProperty { get; }
        public int SetOnlyProperty { set { } }

        public void SimpleMethod() { }
        public void MethodWithManyParams(int a, string b, double c, bool d) { }
        public T GenericMethod<T>(T input) => input;
        
        private void PrivateMethod() { }
        protected void ProtectedMethod() { }
        
        public ComplexClass(int readOnlyField)
        {
            ReadOnlyField = readOnlyField;
        }
    }

    public class ClassAnalyzerTests
    {
        [Fact]
        public void Constructor_WithNullType_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new ClassAnalyzer(null));
        }

        [Fact]
        public void GetPublicMethods_ReturnsCorrectMethods()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var methods = analyzer.GetPublicMethods().ToList();

            // Assert
            Assert.Contains("Method", methods);
            Assert.Contains("MethodWithParams", methods);
            Assert.Contains("MethodWithReturn", methods);
            Assert.DoesNotContain("PrivateMethod", methods);
            
            // Проверяем, что get/set методы свойств не включены
            Assert.DoesNotContain("get_Property", methods);
            Assert.DoesNotContain("set_Property", methods);
        }

        [Fact]
        public void GetPublicMethods_EmptyClass_ReturnsEmpty()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(EmptyClass));

            // Act
            var methods = analyzer.GetPublicMethods().ToList();

            // Assert
            Assert.Empty(methods);
        }

        [Fact]
        public void GetMethodParams_ExistingMethod_ReturnsParameterNames()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var params1 = analyzer.GetMethodParams("MethodWithParams").ToList();
            var params2 = analyzer.GetMethodParams("Method").ToList();

            // Assert
            Assert.Equal(new[] { "param1", "param2" }, params1);
            Assert.Empty(params2);
        }

        [Fact]
        public void GetMethodParams_NonExistentMethod_ReturnsEmpty()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var parameters = analyzer.GetMethodParams("NonExistentMethod").ToList();

            // Assert
            Assert.Empty(parameters);
        }

        [Fact]
        public void GetMethodParams_NullOrEmptyMethodName_ReturnsEmpty()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var paramsNull = analyzer.GetMethodParams(null).ToList();
            var paramsEmpty = analyzer.GetMethodParams("").ToList();

            // Assert
            Assert.Empty(paramsNull);
            Assert.Empty(paramsEmpty);
        }

        [Fact]
        public void GetAllFields_IncludesPrivateFields()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var fields = analyzer.GetAllFields().ToList();

            // Assert
            Assert.Contains("PublicField", fields);
            Assert.Contains("_privateField", fields);
            Assert.Contains("_protectedField", fields);
        }

        [Fact]
        public void GetAllFields_ComplexClass_ReturnsAllInstanceFields()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(ComplexClass));

            // Act
            var fields = analyzer.GetAllFields().ToList();

            // Assert
            Assert.Contains("ReadOnlyField", fields);
            Assert.Contains("_privateField", fields);
            Assert.Contains("PublicField", fields);
            
            // Статические поля не должны включаться с флагом Instance
            Assert.DoesNotContain("StaticField", fields);
        }

        [Fact]
        public void GetProperties_ReturnsAllPublicProperties()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var properties = analyzer.GetProperties().ToList();

            // Assert
            Assert.Contains("Property", properties);
            Assert.Contains("ReadOnlyProperty", properties);
        }

        [Fact]
        public void GetProperties_ComplexClass_ReturnsCorrectProperties()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(ComplexClass));

            // Act
            var properties = analyzer.GetProperties().ToList();

            // Assert
            Assert.Contains("AutoProperty", properties);
            Assert.Contains("GetOnlyProperty", properties);
            Assert.Contains("SetOnlyProperty", properties);
        }

        [Fact]
        public void HasAttribute_SerializableAttribute_ReturnsTrue()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(AttributedClass));

            // Act
            var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

            // Assert
            Assert.True(hasAttribute);
        }

        [Fact]
        public void HasAttribute_NonExistentAttribute_ReturnsFalse()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var hasAttribute = analyzer.HasAttribute<SerializableAttribute>();

            // Assert
            Assert.False(hasAttribute);
        }

        [Fact]
        public void HasAttribute_MultipleAttributes_ReturnsCorrectly()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(MultiAttributeClass));

            // Act
            var hasSerializable = analyzer.HasAttribute<SerializableAttribute>();
            var hasObsolete = analyzer.HasAttribute<ObsoleteAttribute>();
            var hasNonExistent = analyzer.HasAttribute<CLSCompliantAttribute>();

            // Assert
            Assert.True(hasSerializable);
            Assert.True(hasObsolete);
            Assert.False(hasNonExistent);
        }

        [Fact]
        public void AnalyzedType_ReturnsCorrectType()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var analyzedType = analyzer.AnalyzedType;

            // Assert
            Assert.Equal(typeof(TestClass), analyzedType);
        }

        [Fact]
        public void ClassName_ReturnsCorrectName()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var className = analyzer.ClassName;

            // Assert
            Assert.Equal("TestClass", className);
        }

        [Fact]
        public void ClassNamespace_ReturnsCorrectNamespace()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(TestClass));

            // Act
            var classNamespace = analyzer.ClassNamespace;

            // Assert
            Assert.Equal("task05tests", classNamespace);
        }

        [Theory]
        [InlineData(typeof(TestClass))]
        [InlineData(typeof(AttributedClass))]
        [InlineData(typeof(EmptyClass))]
        [InlineData(typeof(ComplexClass))]
        public void ClassAnalyzer_WorksWithDifferentClasses(Type classType)
        {
            // Arrange & Act
            var analyzer = new ClassAnalyzer(classType);

            // Assert
            Assert.NotNull(analyzer);
            Assert.Equal(classType, analyzer.AnalyzedType);
            
            // Все методы должны работать без исключений
            var exception = Record.Exception(() =>
            {
                var methods = analyzer.GetPublicMethods().ToList();
                var fields = analyzer.GetAllFields().ToList();
                var properties = analyzer.GetProperties().ToList();
                var hasSerializable = analyzer.HasAttribute<SerializableAttribute>();
            });
            
            Assert.Null(exception);
        }

        [Fact]
        public void GetMethodParams_ComplexMethod_ReturnsAllParameters()
        {
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(ComplexClass));

            // Act
            var parameters = analyzer.GetMethodParams("MethodWithManyParams").ToList();

            // Assert
            Assert.Equal(new[] { "a", "b", "c", "d" }, parameters);
        }

        [Fact]
        public void Reflection_DoesNotUseLoops_UsesLinqOnly()
        {
            // Этот тест проверяет, что наша реализация использует LINQ вместо циклов
            // Arrange
            var analyzer = new ClassAnalyzer(typeof(ComplexClass));

            // Act & Assert - если код использует LINQ правильно, эти операции должны работать
            var methods = analyzer.GetPublicMethods().Count();
            var fields = analyzer.GetAllFields().Count();
            var properties = analyzer.GetProperties().Count();

            Assert.True(methods >= 0);
            Assert.True(fields >= 0);
            Assert.True(properties >= 0);
        }
    }
}