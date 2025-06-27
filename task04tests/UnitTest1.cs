using Xunit;
using Moq;
using task04;
using System;
using System.IO;

namespace task04tests
{
    public class SpaceshipTests
    {
        [Fact]
        public void Cruiser_ShouldHaveCorrectStats()
        {
            // Arrange & Act
            ISpaceship cruiser = new Cruiser();

            // Assert
            Assert.Equal(50, cruiser.Speed);
            Assert.Equal(100, cruiser.FirePower);
        }

        [Fact]
        public void Fighter_ShouldHaveCorrectStats()
        {
            // Arrange & Act
            ISpaceship fighter = new Fighter();

            // Assert
            Assert.Equal(100, fighter.Speed);
            Assert.Equal(50, fighter.FirePower);
        }

        [Fact]
        public void Fighter_ShouldBeFasterThanCruiser()
        {
            // Arrange
            var fighter = new Fighter();
            var cruiser = new Cruiser();

            // Act & Assert
            Assert.True(fighter.Speed > cruiser.Speed);
        }

        [Fact]
        public void Cruiser_ShouldHaveMoreFirePowerThanFighter()
        {
            // Arrange
            var fighter = new Fighter();
            var cruiser = new Cruiser();

            // Act & Assert
            Assert.True(cruiser.FirePower > fighter.FirePower);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(90)]
        [InlineData(180)]
        [InlineData(270)]
        [InlineData(-90)]
        [InlineData(450)] // 450 должно стать 90
        public void Cruiser_Rotate_ShouldUpdateAngleCorrectly(int rotationAngle)
        {
            // Arrange
            var cruiser = new Cruiser();
            cruiser.Reset(); // Сброс к начальному состоянию

            // Act
            cruiser.Rotate(rotationAngle);

            // Assert
            int expectedAngle = ((rotationAngle % 360) + 360) % 360;
            Assert.Equal(expectedAngle, cruiser.CurrentAngle);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(90)]
        [InlineData(180)]
        [InlineData(270)]
        [InlineData(-90)]
        [InlineData(450)] // 450 должно стать 90
        public void Fighter_Rotate_ShouldUpdateAngleCorrectly(int rotationAngle)
        {
            // Arrange
            var fighter = new Fighter();
            fighter.Reset(); // Сброс к начальному состоянию

            // Act
            fighter.Rotate(rotationAngle);

            // Assert
            int expectedAngle = ((rotationAngle % 360) + 360) % 360;
            Assert.Equal(expectedAngle, fighter.CurrentAngle);
        }

        [Fact]
        public void Cruiser_MoveForward_ShouldUpdatePosition()
        {
            // Arrange
            var cruiser = new Cruiser();
            cruiser.Reset();
            int initialX = cruiser.PositionX;
            int initialY = cruiser.PositionY;

            // Act
            cruiser.MoveForward();

            // Assert
            // При угле 0° корабль должен двигаться по оси X
            Assert.Equal(initialX + cruiser.Speed, cruiser.PositionX);
            Assert.Equal(initialY, cruiser.PositionY);
        }

        [Fact]
        public void Fighter_MoveForward_ShouldUpdatePosition()
        {
            // Arrange
            var fighter = new Fighter();
            fighter.Reset();
            int initialX = fighter.PositionX;
            int initialY = fighter.PositionY;

            // Act
            fighter.MoveForward();

            // Assert
            // При угле 0° корабль должен двигаться по оси X
            Assert.Equal(initialX + fighter.Speed, fighter.PositionX);
            Assert.Equal(initialY, fighter.PositionY);
        }

        [Fact]
        public void Cruiser_MoveForwardAfterRotation_ShouldMoveInCorrectDirection()
        {
            // Arrange
            var cruiser = new Cruiser();
            cruiser.Reset();
            
            // Act
            cruiser.Rotate(90); // Поворот на 90° (движение по оси Y)
            cruiser.MoveForward();

            // Assert
            // При угле 90° корабль должен двигаться по оси Y
            Assert.Equal(0, cruiser.PositionX);
            Assert.Equal(cruiser.Speed, cruiser.PositionY);
        }

        [Fact]
        public void Fighter_Fire_ShouldNotThrowException()
        {
            // Arrange
            var fighter = new Fighter();

            // Act & Assert
            var exception = Record.Exception(() => fighter.Fire());
            Assert.Null(exception);
        }

        [Fact]
        public void Cruiser_Fire_ShouldNotThrowException()
        {
            // Arrange
            var cruiser = new Cruiser();

            // Act & Assert
            var exception = Record.Exception(() => cruiser.Fire());
            Assert.Null(exception);
        }

        [Fact]
        public void BothShips_ShouldImplementISpaceshipInterface()
        {
            // Arrange & Act
            ISpaceship fighter = new Fighter();
            ISpaceship cruiser = new Cruiser();

            // Assert
            Assert.IsAssignableFrom<ISpaceship>(fighter);
            Assert.IsAssignableFrom<ISpaceship>(cruiser);
        }

        [Fact]
        public void MultipleRotations_ShouldAccumulate()
        {
            // Arrange
            var fighter = new Fighter();
            fighter.Reset();

            // Act
            fighter.Rotate(45);
            fighter.Rotate(45);
            fighter.Rotate(45);

            // Assert
            Assert.Equal(135, fighter.CurrentAngle);
        }

        [Fact]
        public void Ships_ShouldHaveConsistentBehavior()
        {
            // Arrange
            var ships = new ISpaceship[] { new Fighter(), new Cruiser() };

            // Act & Assert
            foreach (var ship in ships)
            {
                // Все корабли должны иметь положительную скорость и мощность
                Assert.True(ship.Speed > 0);
                Assert.True(ship.FirePower > 0);

                // Все методы должны работать без исключений
                var exception = Record.Exception(() =>
                {
                    ship.MoveForward();
                    ship.Rotate(90);
                    ship.Fire();
                });
                Assert.Null(exception);
            }
        }

        [Fact]
        public void Reset_ShouldResetShipToInitialState()
        {
            // Arrange
            var cruiser = new Cruiser();
            cruiser.MoveForward();
            cruiser.Rotate(90);

            // Act
            cruiser.Reset();

            // Assert
            Assert.Equal(0, cruiser.CurrentAngle);
            Assert.Equal(0, cruiser.PositionX);
            Assert.Equal(0, cruiser.PositionY);
        }
    }
}