using System;

namespace task04
{
    /// <summary>
    /// Крейсер - медленный корабль с мощными ракетами
    /// </summary>
    public class Cruiser : ISpaceship
    {
        private int _currentAngle = 0;
        private int _positionX = 0;
        private int _positionY = 0;

        /// <summary>
        /// Скорость крейсера (медленная)
        /// </summary>
        public int Speed => 50;

        /// <summary>
        /// Мощность выстрела крейсера (высокая)
        /// </summary>
        public int FirePower => 100;

        /// <summary>
        /// Текущий угол поворота корабля
        /// </summary>
        public int CurrentAngle => _currentAngle;

        /// <summary>
        /// Текущая позиция X
        /// </summary>
        public int PositionX => _positionX;

        /// <summary>
        /// Текущая позиция Y
        /// </summary>
        public int PositionY => _positionY;

        /// <summary>
        /// Движение вперед со скоростью крейсера
        /// </summary>
        public void MoveForward()
        {
            // Вычисляем новую позицию на основе текущего угла и скорости
            double radians = _currentAngle * Math.PI / 180.0;
            _positionX += (int)(Speed * Math.Cos(radians));
            _positionY += (int)(Speed * Math.Sin(radians));
            
            Console.WriteLine($"Cruiser moved forward to position ({_positionX}, {_positionY})");
        }

        /// <summary>
        /// Поворот крейсера на указанный угол
        /// </summary>
        /// <param name="angle">Угол поворота в градусах</param>
        public void Rotate(int angle)
        {
            _currentAngle = (_currentAngle + angle) % 360;
            if (_currentAngle < 0) _currentAngle += 360;
            
            Console.WriteLine($"Cruiser rotated to angle {_currentAngle}°");
        }

        /// <summary>
        /// Выстрел мощной ракетой
        /// </summary>
        public void Fire()
        {
            Console.WriteLine($"Cruiser fires powerful photon missile! Power: {FirePower}");
        }

        /// <summary>
        /// Сброс позиции и угла корабля
        /// </summary>
        public void Reset()
        {
            _currentAngle = 0;
            _positionX = 0;
            _positionY = 0;
        }
    }
}