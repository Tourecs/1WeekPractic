namespace CommandLib
{
    /// <summary>
    /// Интерфейс команды для паттерна Command
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Выполняет команду
        /// </summary>
        void Execute();

        /// <summary>
        /// Описание команды
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Имя команды
        /// </summary>
        string Name { get; }
    }
}