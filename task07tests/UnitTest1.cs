using System;
using System.Linq;
using System.Reflection;
using Xunit;
using task07;

namespace task07tests
{
    public class AttributeReflectionTests
    {
        [Fact]
        public void Class_HasDisplayNameAttribute()
        {
            // Act
            var type = typeof(SampleClass);
            var attribute = type.GetCustomAttribute<DisplayNameAttribute>();
            
            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Пример класса", attribute.DisplayName);
        }

        [Fact]
        public void Method_HasDisplayNameAttribute()
        {
            // Act
            var method = typeof(SampleClass).GetMethod("TestMethod");
            var attribute = method.GetCustomAttribute<DisplayNameAttribute>();
            
            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Тестовый метод", attribute.DisplayName);
        }

        [Fact]
        public void Property_HasDisplayNameAttribute()
        {
            // Act
            var prop = typeof(SampleClass).GetProperty("Number");
            var attribute = prop.GetCustomAttribute<DisplayNameAttribute>();
            
            // Assert
            Assert.NotNull(attribute);
            Assert.Equal("Числовое свойство", attribute.DisplayName);
        }

        [Fact]
        public void Class_HasVersionAttribute()
        {
            // Act
            var type = typeof(SampleClass);
            var attribute = type.GetCustomAttribute<VersionAttribute>();
            
            // Assert
            Assert.NotNull(attribute);
            Assert.Equal(1, attribute.Major);
            Assert.Equal(0, attribute.Minor);
        }

        [Fact]
        public void DisplayNameAttribute_Constructor_SetsDisplayName()
        {
            // Arrange
            const string expectedName = "Test Name";
            
            // Act
            var attribute = new DisplayNameAttribute(expectedName);
            
            // Assert
            Assert.Equal(expectedName, attribute.DisplayName);
        }

        [Fact]
        public void DisplayNameAttribute_Constructor_ThrowsOnNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DisplayNameAttribute(null));
        }

        [Fact]
        public void VersionAttribute_Constructor_SetsVersions()
        {
            // Act
            var attribute = new VersionAttribute(2, 5);
            
            // Assert
            Assert.Equal(2, attribute.Major);
            Assert.Equal(5, attribute.Minor);
            Assert.Equal("2.5", attribute.Version);
        }

        [Fact]
        public void VersionAttribute_StringConstructor_ParsesVersionCorrectly()
        {
            // Act
            var attribute = new VersionAttribute("3.14");
            
            // Assert
            Assert.Equal(3, attribute.Major);
            Assert.Equal(14, attribute.Minor);
            Assert.Equal("3.14", attribute.Version);
        }

        [Theory]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("1.2.3")]
        [InlineData("a.b")]
        [InlineData("1.-1")]
        public void VersionAttribute_StringConstructor_ThrowsOnInvalidInput(string version)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new VersionAttribute(version));
        }

        [Fact]
        public void VersionAttribute_Constructor_ThrowsOnNegativeValues()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new VersionAttribute(-1, 0));
            Assert.Throws<ArgumentException>(() => new VersionAttribute(0, -1));
        }

        [Fact]
        public void ReflectionHelper_GetTypeInfo_ReturnsValidInfo()
        {
            // Act
            var info = ReflectionHelper.GetTypeInfo(typeof(SampleClass));
            
            // Assert
            Assert.NotNull(info);
            Assert.Contains("АНАЛИЗ ТИПА: SampleClass", info);
            Assert.Contains("Пример класса", info);
            Assert.Contains("Версия: 1.0", info);
        }

        [Fact]
        public void ReflectionHelper_GetTypeInfo_ThrowsOnNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ReflectionHelper.GetTypeInfo(null));
        }

        [Fact]
        public void ReflectionHelper_PrintTypeInfo_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => ReflectionHelper.PrintTypeInfo(typeof(SampleClass)));
            Assert.Null(exception);
        }

        [Fact]
        public void SampleClass_AllAttributedMembersHaveCorrectAttributes()
        {
            // Arrange
            var type = typeof(SampleClass);
            
            // Act & Assert
            
            // Проверяем метод Calculate
            var calculateMethod = type.GetMethod("Calculate");
            var calculateAttr = calculateMethod.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(calculateAttr);
            Assert.Equal("Метод вычисления", calculateAttr.DisplayName);
            
            // Проверяем свойство Name
            var nameProperty = type.GetProperty("Name");
            var nameAttr = nameProperty.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(nameAttr);
            Assert.Equal("Имя объекта", nameAttr.DisplayName);
            
            // Проверяем поле IsActive
            var isActiveField = type.GetField("IsActive");
            var isActiveAttr = isActiveField.GetCustomAttribute<DisplayNameAttribute>();
            Assert.NotNull(isActiveAttr);
            Assert.Equal("Публичное поле", isActiveAttr.DisplayName);
        }

        [Fact]
        public void SampleClass_MembersWithoutAttributes_DoNotHaveDisplayName()
        {
            // Arrange
            var type = typeof(SampleClass);
            
            // Act
            var getInfoMethod = type.GetMethod("GetInfo");
            var createdDateProperty = type.GetProperty("CreatedDate");
            
            // Assert
            Assert.Null(getInfoMethod.GetCustomAttribute<DisplayNameAttribute>());
            Assert.Null(createdDateProperty.GetCustomAttribute<DisplayNameAttribute>());
        }

        [Fact]
        public void ReflectionHelper_GetTypesWithAttribute_FindsCorrectTypes()
        {
            // Act
            var typesWithDisplayName = ReflectionHelper.GetTypesWithAttribute<DisplayNameAttribute>().ToList();
            var typesWithVersion = ReflectionHelper.GetTypesWithAttribute<VersionAttribute>().ToList();
            
            // Assert
            Assert.Contains(typeof(SampleClass), typesWithDisplayName);
            Assert.Contains(typeof(SampleClass), typesWithVersion);
        }

        [Fact]
        public void ReflectionHelper_GetAttributeValue_ReturnsCorrectAttribute()
        {
            // Act
            var displayNameAttr = ReflectionHelper.GetAttributeValue<DisplayNameAttribute>(typeof(SampleClass));
            var versionAttr = ReflectionHelper.GetAttributeValue<VersionAttribute>(typeof(SampleClass));
            
            // Assert
            Assert.NotNull(displayNameAttr);
            Assert.Equal("Пример класса", displayNameAttr.DisplayName);
            
            Assert.NotNull(versionAttr);
            Assert.Equal(1, versionAttr.Major);
            Assert.Equal(0, versionAttr.Minor);
        }

        [Fact]
        public void AttributeEquality_WorksCorrectly()
        {
            // Arrange
            var attr1 = new DisplayNameAttribute("Test");
            var attr2 = new DisplayNameAttribute("Test");
            var attr3 = new DisplayNameAttribute("Different");
            
            var version1 = new VersionAttribute(1, 0);
            var version2 = new VersionAttribute(1, 0);
            var version3 = new VersionAttribute(2, 0);
            
            // Assert
            Assert.Equal(attr1, attr2);
            Assert.NotEqual(attr1, attr3);
            Assert.Equal(attr1.GetHashCode(), attr2.GetHashCode());
            
            Assert.Equal(version1, version2);
            Assert.NotEqual(version1, version3);
            Assert.Equal(version1.GetHashCode(), version2.GetHashCode());
        }

        [Fact]
        public void VersionAttribute_CompareTo_WorksCorrectly()
        {
            // Arrange
            var version1_0 = new VersionAttribute(1, 0);
            var version1_1 = new VersionAttribute(1, 1);
            var version2_0 = new VersionAttribute(2, 0);
            var version1_0_copy = new VersionAttribute(1, 0);
            
            // Act & Assert
            Assert.True(version1_0.CompareTo(version1_1) < 0); // 1.0 < 1.1
            Assert.True(version1_1.CompareTo(version2_0) < 0); // 1.1 < 2.0
            Assert.True(version2_0.CompareTo(version1_0) > 0); // 2.0 > 1.0
            Assert.Equal(0, version1_0.CompareTo(version1_0_copy)); // 1.0 == 1.0
            Assert.True(version1_0.CompareTo(null) > 0); // not null > null
        }

        [Fact]
        public void SampleClass_StaticMethod_HasCorrectAttribute()
        {
            // Arrange
            var type = typeof(SampleClass);
            
            // Act
            var getVersionMethod = type.GetMethod("GetVersion", BindingFlags.Public | BindingFlags.Static);
            var attribute = getVersionMethod.GetCustomAttribute<DisplayNameAttribute>();
            
            // Assert
            Assert.NotNull(getVersionMethod);
            Assert.NotNull(attribute);
            Assert.Equal("Статический метод", attribute.DisplayName);
        }

        [Fact]
        public void SampleClass_PrivateMethod_HasCorrectAttribute()
        {
            // Arrange
            var type = typeof(SampleClass);
            
            // Act
            var privateMethod = type.GetMethod("PrivateMethod", BindingFlags.NonPublic | BindingFlags.Instance);
            var attribute = privateMethod?.GetCustomAttribute<DisplayNameAttribute>();
            
            // Assert
            Assert.NotNull(privateMethod);
            Assert.NotNull(attribute);
            Assert.Equal("Приватный метод", attribute.DisplayName);
        }

        [Fact]
        public void ReflectionHelper_AnalyzesPrivateMembers()
        {
            // Act
            var info = ReflectionHelper.GetTypeInfo(typeof(SampleClass));
            
            // Assert
            Assert.Contains("PrivateMethod", info);
            Assert.Contains("Приватный метод", info);
            Assert.Contains("_internalId", info);
            Assert.Contains("Приватное поле", info);
        }

        [Fact]
        public void AttributeToString_ReturnsCorrectFormat()
        {
            // Arrange
            var displayName = new DisplayNameAttribute("Test Display");
            var version = new VersionAttribute(2, 3);
            
            // Act
            var displayString = displayName.ToString();
            var versionString = version.ToString();
            
            // Assert
            Assert.Equal("DisplayName: Test Display", displayString);
            Assert.Equal("Version: 2.3", versionString);
        }

        [Fact]
        public void SampleClass_FunctionalityWorks()
        {
            // Arrange
            var sample = new SampleClass(5, "Test Object");
            
            // Act
            var result = sample.Calculate(3);
            var info = sample.GetInfo();
            var version = SampleClass.GetVersion();
            
            // Assert
            Assert.Equal(15, result); // 5 * 3
            Assert.Contains("Test Object", info);
            Assert.Contains("5", info);
            Assert.Equal("1.0", version);
        }
    }
}