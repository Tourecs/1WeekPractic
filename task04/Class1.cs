namespace task04
{
    /// <summary>
    /// Интерфейс космического корабля
    /// </summary>
    public interface ISpaceship
    {
        /// <summary>
        /// Движение вперед
        /// </summary>
        void MoveForward();

        /// <summary>
        /// Поворот на указанный угол
        /// </summary>
        /// <param name="angle">Угол поворота в градусах</param>
        void Rotate(int angle);

        /// <summary>
        /// Выстрел ракетой
        /// </summary>
        void Fire();

        /// <summary>
        /// Скорость корабля
        /// </summary>
        int Speed { get; }

        /// <summary>
        /// Мощность выстрела
        /// </summary>
        int FirePower { get; }
    }
}