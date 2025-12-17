using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace SqrtMVVM.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private string _gameStatus = "Готов к игре";
        private string _currentPlayer = "Белые";
        private string _currentPlayerColor = "Белые шашки";
        private string _whiteScore = "Белые: 12";
        private string _blackScore = "Черные: 12";
        private string _gamesPlayed = "Игр сыграно: 0";
        private string _whiteWins = "Побед белых: 0";
        private string _blackWins = "Побед черных: 0";
        private string _timerWhite = "Белые: 10:00";
        private string _timerBlack = "Черные: 10:00";

        // Для отображения сообщения о результате игры
        private bool _showGameResult = false;
        private string _gameResultMessage = "";
        private IBrush _gameResultBackground = Brushes.LightGreen;

        private CheckerCell _selectedCell;
        private List<CheckerCell> _cellsWithMandatoryCapture = new List<CheckerCell>();
        private bool _isGameActive = true;
        private IBrush _boardBackground = Brushes.BurlyWood;
        private int _boardSize = 8;
        private Stack<List<CheckerCell>> _moveHistory = new Stack<List<CheckerCell>>();

        // Для отслеживания множественного взятия
        private bool _isMultipleCapture = false;
        private CheckerCell _currentCaptureCell = null;
        private List<CheckerCell> _capturedCellsInCurrentMove = new List<CheckerCell>();

        // Видимость меню
        private bool _isGameMenuVisible;
        private bool _isSettingsMenuVisible;
        private bool _isModeMenuVisible;
        private bool _isAppearanceMenuVisible;
        private bool _isStatsMenuVisible;

        // Отслеживание активного меню
        private string _activeMenu = string.Empty;

        // Фоны для активных вкладок
        private IBrush _gameTabBackground = Brushes.Transparent;
        private IBrush _settingsTabBackground = Brushes.Transparent;
        private IBrush _appearanceTabBackground = Brushes.Transparent;
        private IBrush _modeTabBackground = Brushes.Transparent;
        private IBrush _statsTabBackground = Brushes.Transparent;

        // Состояния настроек
        private bool _isSoundEnabled = true;
        private bool _areHintsEnabled = true;
        private bool _isTimerEnabled = false;

        // Уровень бота и размер доски
        private int _botLevel = 1;
        private int _selectedBoardSize = 8;

        // Режим игры
        private string _gameMode = "PlayerVsPlayer";

        // Фоны для выделения выбранных опций
        private IBrush _playerVsPlayerBackground = Brushes.LightGreen;
        private IBrush _playerVsComputerBackground = Brushes.Transparent;
        private IBrush _computerVsComputerBackground = Brushes.Transparent;

        private IBrush _classicColorsBackground = Brushes.LightGreen;
        private IBrush _redColorsBackground = Brushes.Transparent;
        private IBrush _blueColorsBackground = Brushes.Transparent;

        private IBrush _classicBoardBackground = Brushes.LightGreen;
        private IBrush _greenBoardBackground = Brushes.Transparent;
        private IBrush _grayBoardBackground = Brushes.Transparent;

        // Фоны для кнопок настроек
        private IBrush _soundButtonBackground = Brushes.LightGreen;
        private IBrush _hintsButtonBackground = Brushes.LightGreen;
        private IBrush _timerButtonBackground = Brushes.Transparent;

        // Фоны для уровней бота
        private IBrush _botLevel1Background = Brushes.LightGreen;
        private IBrush _botLevel2Background = Brushes.Transparent;
        private IBrush _botLevel3Background = Brushes.Transparent;
        private IBrush _botLevel4Background = Brushes.Transparent;

        // Фоны для размеров доски
        private IBrush _boardSize8Background = Brushes.LightGreen;
        private IBrush _boardSize9Background = Brushes.Transparent;
        private IBrush _boardSize10Background = Brushes.Transparent;

        // Видимость элементов бота
        private bool _isBotSettingsEnabled = false;

        public MainWindowViewModel()
        {
            BoardCells = new ObservableCollection<CheckerCell>();
            InitializeBoard();
            InitializeCommands();
            CheckMandatoryCaptures();
            _capturedCellsInCurrentMove = new List<CheckerCell>();
        }

        public ObservableCollection<CheckerCell> BoardCells { get; }

        public string GameStatus
        {
            get => _gameStatus;
            set => this.RaiseAndSetIfChanged(ref _gameStatus, value);
        }

        public string CurrentPlayer
        {
            get => _currentPlayer;
            set => this.RaiseAndSetIfChanged(ref _currentPlayer, value);
        }

        public string CurrentPlayerColor
        {
            get => _currentPlayerColor;
            set => this.RaiseAndSetIfChanged(ref _currentPlayerColor, value);
        }

        public string WhiteScore
        {
            get => _whiteScore;
            set => this.RaiseAndSetIfChanged(ref _whiteScore, value);
        }

        public string BlackScore
        {
            get => _blackScore;
            set => this.RaiseAndSetIfChanged(ref _blackScore, value);
        }

        public string GamesPlayed
        {
            get => _gamesPlayed;
            set => this.RaiseAndSetIfChanged(ref _gamesPlayed, value);
        }

        public string WhiteWins
        {
            get => _whiteWins;
            set => this.RaiseAndSetIfChanged(ref _whiteWins, value);
        }

        public string BlackWins
        {
            get => _blackWins;
            set => this.RaiseAndSetIfChanged(ref _blackWins, value);
        }

        public string TimerWhite
        {
            get => _timerWhite;
            set => this.RaiseAndSetIfChanged(ref _timerWhite, value);
        }

        public string TimerBlack
        {
            get => _timerBlack;
            set => this.RaiseAndSetIfChanged(ref _timerBlack, value);
        }

        // Для отображения результата игры
        public bool ShowGameResult
        {
            get => _showGameResult;
            set => this.RaiseAndSetIfChanged(ref _showGameResult, value);
        }

        public string GameResultMessage
        {
            get => _gameResultMessage;
            set => this.RaiseAndSetIfChanged(ref _gameResultMessage, value);
        }

        public IBrush GameResultBackground
        {
            get => _gameResultBackground;
            set => this.RaiseAndSetIfChanged(ref _gameResultBackground, value);
        }

        public CheckerCell SelectedCell
        {
            get => _selectedCell;
            set => this.RaiseAndSetIfChanged(ref _selectedCell, value);
        }

        public bool IsGameActive
        {
            get => _isGameActive;
            set => this.RaiseAndSetIfChanged(ref _isGameActive, value);
        }

        public bool IsTimerEnabled
        {
            get => _isTimerEnabled;
            set => this.RaiseAndSetIfChanged(ref _isTimerEnabled, value);
        }

        public IBrush BoardBackground
        {
            get => _boardBackground;
            set => this.RaiseAndSetIfChanged(ref _boardBackground, value);
        }

        public int BoardSize
        {
            get => _boardSize;
            set => this.RaiseAndSetIfChanged(ref _boardSize, value);
        }

        // Видимость меню
        public bool IsGameMenuVisible
        {
            get => _isGameMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _isGameMenuVisible, value);
        }

        public bool IsSettingsMenuVisible
        {
            get => _isSettingsMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _isSettingsMenuVisible, value);
        }

        public bool IsModeMenuVisible
        {
            get => _isModeMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _isModeMenuVisible, value);
        }

        public bool IsAppearanceMenuVisible
        {
            get => _isAppearanceMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _isAppearanceMenuVisible, value);
        }

        public bool IsStatsMenuVisible
        {
            get => _isStatsMenuVisible;
            set => this.RaiseAndSetIfChanged(ref _isStatsMenuVisible, value);
        }

        // Фоны для активных вкладок
        public IBrush GameTabBackground
        {
            get => _gameTabBackground;
            set => this.RaiseAndSetIfChanged(ref _gameTabBackground, value);
        }

        public IBrush SettingsTabBackground
        {
            get => _settingsTabBackground;
            set => this.RaiseAndSetIfChanged(ref _settingsTabBackground, value);
        }

        public IBrush AppearanceTabBackground
        {
            get => _appearanceTabBackground;
            set => this.RaiseAndSetIfChanged(ref _appearanceTabBackground, value);
        }

        public IBrush ModeTabBackground
        {
            get => _modeTabBackground;
            set => this.RaiseAndSetIfChanged(ref _modeTabBackground, value);
        }

        public IBrush StatsTabBackground
        {
            get => _statsTabBackground;
            set => this.RaiseAndSetIfChanged(ref _statsTabBackground, value);
        }

        // Состояния настроек
        public bool IsSoundEnabled
        {
            get => _isSoundEnabled;
            set => this.RaiseAndSetIfChanged(ref _isSoundEnabled, value);
        }

        public bool AreHintsEnabled
        {
            get => _areHintsEnabled;
            set => this.RaiseAndSetIfChanged(ref _areHintsEnabled, value);
        }

        // Уровень бота и размер доски
        public int BotLevel
        {
            get => _botLevel;
            set => this.RaiseAndSetIfChanged(ref _botLevel, value);
        }

        public int SelectedBoardSize
        {
            get => _selectedBoardSize;
            set => this.RaiseAndSetIfChanged(ref _selectedBoardSize, value);
        }

        // Режим игры
        public string GameMode
        {
            get => _gameMode;
            set => this.RaiseAndSetIfChanged(ref _gameMode, value);
        }

        // Фоны для выделения выбранных опций
        public IBrush PlayerVsPlayerBackground
        {
            get => _playerVsPlayerBackground;
            set => this.RaiseAndSetIfChanged(ref _playerVsPlayerBackground, value);
        }

        public IBrush PlayerVsComputerBackground
        {
            get => _playerVsComputerBackground;
            set => this.RaiseAndSetIfChanged(ref _playerVsComputerBackground, value);
        }

        public IBrush ComputerVsComputerBackground
        {
            get => _computerVsComputerBackground;
            set => this.RaiseAndSetIfChanged(ref _computerVsComputerBackground, value);
        }

        public IBrush ClassicColorsBackground
        {
            get => _classicColorsBackground;
            set => this.RaiseAndSetIfChanged(ref _classicColorsBackground, value);
        }

        public IBrush RedColorsBackground
        {
            get => _redColorsBackground;
            set => this.RaiseAndSetIfChanged(ref _redColorsBackground, value);
        }

        public IBrush BlueColorsBackground
        {
            get => _blueColorsBackground;
            set => this.RaiseAndSetIfChanged(ref _blueColorsBackground, value);
        }

        public IBrush ClassicBoardBackground
        {
            get => _classicBoardBackground;
            set => this.RaiseAndSetIfChanged(ref _classicBoardBackground, value);
        }

        public IBrush GreenBoardBackground
        {
            get => _greenBoardBackground;
            set => this.RaiseAndSetIfChanged(ref _greenBoardBackground, value);
        }

        public IBrush GrayBoardBackground
        {
            get => _grayBoardBackground;
            set => this.RaiseAndSetIfChanged(ref _grayBoardBackground, value);
        }

        // Фоны для кнопок настроек
        public IBrush SoundButtonBackground
        {
            get => _soundButtonBackground;
            set => this.RaiseAndSetIfChanged(ref _soundButtonBackground, value);
        }

        public IBrush HintsButtonBackground
        {
            get => _hintsButtonBackground;
            set => this.RaiseAndSetIfChanged(ref _hintsButtonBackground, value);
        }

        public IBrush TimerButtonBackground
        {
            get => _timerButtonBackground;
            set => this.RaiseAndSetIfChanged(ref _timerButtonBackground, value);
        }

        // Фоны для уровней бота
        public IBrush BotLevel1Background
        {
            get => _botLevel1Background;
            set => this.RaiseAndSetIfChanged(ref _botLevel1Background, value);
        }

        public IBrush BotLevel2Background
        {
            get => _botLevel2Background;
            set => this.RaiseAndSetIfChanged(ref _botLevel2Background, value);
        }

        public IBrush BotLevel3Background
        {
            get => _botLevel3Background;
            set => this.RaiseAndSetIfChanged(ref _botLevel3Background, value);
        }

        public IBrush BotLevel4Background
        {
            get => _botLevel4Background;
            set => this.RaiseAndSetIfChanged(ref _botLevel4Background, value);
        }

        // Фоны для размеров доски
        public IBrush BoardSize8Background
        {
            get => _boardSize8Background;
            set => this.RaiseAndSetIfChanged(ref _boardSize8Background, value);
        }

        public IBrush BoardSize9Background
        {
            get => _boardSize9Background;
            set => this.RaiseAndSetIfChanged(ref _boardSize9Background, value);
        }

        public IBrush BoardSize10Background
        {
            get => _boardSize10Background;
            set => this.RaiseAndSetIfChanged(ref _boardSize10Background, value);
        }

        // Видимость элементов бота
        public bool IsBotSettingsEnabled
        {
            get => _isBotSettingsEnabled;
            set => this.RaiseAndSetIfChanged(ref _isBotSettingsEnabled, value);
        }

        // Команды для уровней бота
        public ICommand SetBotLevel1Command { get; private set; }
        public ICommand SetBotLevel2Command { get; private set; }
        public ICommand SetBotLevel3Command { get; private set; }
        public ICommand SetBotLevel4Command { get; private set; }

        // Команды для размеров доски
        public ICommand SetBoardSize8Command { get; private set; }
        public ICommand SetBoardSize9Command { get; private set; }
        public ICommand SetBoardSize10Command { get; private set; }

        // Остальные команды...
        public ICommand NewGameCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }
        public ICommand ExitCommand { get; private set; }
        public ICommand SetPlayerVsPlayerCommand { get; private set; }
        public ICommand SetPlayerVsComputerCommand { get; private set; }
        public ICommand SetComputerVsComputerCommand { get; private set; }
        public ICommand ShowRulesCommand { get; private set; }
        public ICommand AboutCommand { get; private set; }
        public ICommand EndGameCommand { get; private set; }
        public ICommand PauseGameCommand { get; private set; }
        public ICommand SaveGameCommand { get; private set; }
        public ICommand LoadGameCommand { get; private set; }
        public ICommand ToggleSoundCommand { get; private set; }
        public ICommand ToggleHintsCommand { get; private set; }
        public ICommand ToggleTimerCommand { get; private set; }
        public ICommand ToggleDifficultyCommand { get; private set; }
        public ICommand ToggleBoardSizeCommand { get; private set; }
        public ICommand SetClassicColorsCommand { get; private set; }
        public ICommand SetRedColorsCommand { get; private set; }
        public ICommand SetBlueColorsCommand { get; private set; }
        public ICommand SetClassicBoardCommand { get; private set; }
        public ICommand SetGreenBoardCommand { get; private set; }
        public ICommand SetGrayBoardCommand { get; private set; }
        public ICommand ShowGameHistoryCommand { get; private set; }
        public ICommand ResetStatsCommand { get; private set; }
        public ICommand ExportStatsCommand { get; private set; }
        public ICommand ToggleGameMenuCommand { get; private set; }
        public ICommand ToggleSettingsMenuCommand { get; private set; }
        public ICommand ToggleModeMenuCommand { get; private set; }
        public ICommand ToggleAppearanceMenuCommand { get; private set; }
        public ICommand ToggleStatsMenuCommand { get; private set; }
        public ICommand CloseGameResultCommand { get; private set; }

        private void InitializeCommands()
        {
            // Основные команды игры
            NewGameCommand = ReactiveCommand.Create(NewGame);
            UndoCommand = ReactiveCommand.Create(UndoMove);
            ResetCommand = ReactiveCommand.Create(ResetGame);
            ExitCommand = ReactiveCommand.Create(ExitGame);
            EndGameCommand = ReactiveCommand.Create(EndGame);
            PauseGameCommand = ReactiveCommand.Create(PauseGame);
            SaveGameCommand = ReactiveCommand.Create(SaveGame);
            LoadGameCommand = ReactiveCommand.Create(LoadGame);
            CloseGameResultCommand = ReactiveCommand.Create(CloseGameResult);

            // Команды режима игры
            SetPlayerVsPlayerCommand = ReactiveCommand.Create(SetPlayerVsPlayer);
            SetPlayerVsComputerCommand = ReactiveCommand.Create(SetPlayerVsComputer);
            SetComputerVsComputerCommand = ReactiveCommand.Create(SetComputerVsComputer);

            // Команды уровней бота
            SetBotLevel1Command = ReactiveCommand.Create(() => SetBotLevel(1));
            SetBotLevel2Command = ReactiveCommand.Create(() => SetBotLevel(2));
            SetBotLevel3Command = ReactiveCommand.Create(() => SetBotLevel(3));
            SetBotLevel4Command = ReactiveCommand.Create(() => SetBotLevel(4));

            // Команды размеров доски
            SetBoardSize8Command = ReactiveCommand.Create(() => SetBoardSize(8));
            SetBoardSize9Command = ReactiveCommand.Create(() => SetBoardSize(9));
            SetBoardSize10Command = ReactiveCommand.Create(() => SetBoardSize(10));

            // Остальные команды...
            ShowRulesCommand = ReactiveCommand.Create(ShowRules);
            AboutCommand = ReactiveCommand.Create(ShowAbout);
            ToggleSoundCommand = ReactiveCommand.Create(ToggleSound);
            ToggleHintsCommand = ReactiveCommand.Create(ToggleHints);
            ToggleTimerCommand = ReactiveCommand.Create(ToggleTimer);
            ToggleDifficultyCommand = ReactiveCommand.Create(ToggleDifficulty);
            ToggleBoardSizeCommand = ReactiveCommand.Create(ToggleBoardSize);
            SetClassicColorsCommand = ReactiveCommand.Create(SetClassicColors);
            SetRedColorsCommand = ReactiveCommand.Create(SetRedColors);
            SetBlueColorsCommand = ReactiveCommand.Create(SetBlueColors);
            SetClassicBoardCommand = ReactiveCommand.Create(SetClassicBoard);
            SetGreenBoardCommand = ReactiveCommand.Create(SetGreenBoard);
            SetGrayBoardCommand = ReactiveCommand.Create(SetGrayBoard);
            ShowGameHistoryCommand = ReactiveCommand.Create(ShowGameHistory);
            ResetStatsCommand = ReactiveCommand.Create(ResetStats);
            ExportStatsCommand = ReactiveCommand.Create(ExportStats);
            ToggleGameMenuCommand = ReactiveCommand.Create(() => ToggleMenu(nameof(IsGameMenuVisible)));
            ToggleSettingsMenuCommand = ReactiveCommand.Create(() => ToggleMenu(nameof(IsSettingsMenuVisible)));
            ToggleModeMenuCommand = ReactiveCommand.Create(() => ToggleMenu(nameof(IsModeMenuVisible)));
            ToggleAppearanceMenuCommand = ReactiveCommand.Create(() => ToggleMenu(nameof(IsAppearanceMenuVisible)));
            ToggleStatsMenuCommand = ReactiveCommand.Create(() => ToggleMenu(nameof(IsStatsMenuVisible)));
        }

        private void InitializeBoard()
        {
            BoardCells.Clear();
            int size = BoardSize;

            for (int row = 0; row < size; row++)
            {
                for (int col = 0; col < size; col++)
                {
                    var cell = new CheckerCell
                    {
                        Row = row,
                        Column = col,
                        Background = (row + col) % 2 == 0 ? Brushes.BurlyWood : Brushes.SaddleBrown,
                        HasChecker = false,
                        CheckerFill = Brushes.Transparent,
                        IsKing = false,
                        IsSelected = false,
                        IsPossibleMove = false,
                        IsMandatoryCapture = false
                    };

                    // Расстановка начальных шашек (только на черных клетках)
                    if ((row + col) % 2 == 1)
                    {
                        int checkerRows = size == 8 ? 3 : size == 9 ? 3 : 4;
                        if (row < checkerRows)
                        {
                            // Черные шашки вверху
                            cell.HasChecker = true;
                            cell.CheckerFill = Brushes.Black;
                        }
                        else if (row >= size - checkerRows)
                        {
                            // Белые шашки внизу
                            cell.HasChecker = true;
                            cell.CheckerFill = Brushes.White;
                        }
                    }

                    BoardCells.Add(cell);
                }
            }

            UpdateScores();
            _capturedCellsInCurrentMove.Clear();
        }

        // Методы для уровней бота
        private void SetBotLevel(int level)
        {
            BotLevel = level;

            // Сбрасываем все фоны
            BotLevel1Background = Brushes.Transparent;
            BotLevel2Background = Brushes.Transparent;
            BotLevel3Background = Brushes.Transparent;
            BotLevel4Background = Brushes.Transparent;

            // Устанавливаем фон для выбранного уровня
            switch (level)
            {
                case 1: BotLevel1Background = Brushes.LightGreen; break;
                case 2: BotLevel2Background = Brushes.LightGreen; break;
                case 3: BotLevel3Background = Brushes.LightGreen; break;
                case 4: BotLevel4Background = Brushes.LightGreen; break;
            }

            GameStatus = $"Уровень бота установлен: {level}";
            CloseAllMenus();
        }

        // Методы для размеров доски
        private void SetBoardSize(int size)
        {
            SelectedBoardSize = size;
            BoardSize = size;

            // Сбрасываем все фоны
            BoardSize8Background = Brushes.Transparent;
            BoardSize9Background = Brushes.Transparent;
            BoardSize10Background = Brushes.Transparent;

            // Устанавливаем фон для выбранного размера
            switch (size)
            {
                case 8: BoardSize8Background = Brushes.LightGreen; break;
                case 9: BoardSize9Background = Brushes.LightGreen; break;
                case 10: BoardSize10Background = Brushes.LightGreen; break;
            }

            InitializeBoard();
            GameStatus = $"Размер доски установлен: {size}x{size}";
            CloseAllMenus();
        }

        // Обновленные методы режимов игры
        private void SetPlayerVsPlayer()
        {
            GameMode = "PlayerVsPlayer";
            IsBotSettingsEnabled = false;
            GameStatus = "Режим: Игрок vs Игрок";
            IsGameActive = true;

            PlayerVsPlayerBackground = Brushes.LightGreen;
            PlayerVsComputerBackground = Brushes.Transparent;
            ComputerVsComputerBackground = Brushes.Transparent;

            CloseAllMenus();
        }

        private void SetPlayerVsComputer()
        {
            GameMode = "PlayerVsComputer";
            IsBotSettingsEnabled = true;
            GameStatus = "Режим: Игрок vs Компьютер";
            IsGameActive = true;

            PlayerVsPlayerBackground = Brushes.Transparent;
            PlayerVsComputerBackground = Brushes.LightGreen;
            ComputerVsComputerBackground = Brushes.Transparent;

            CloseAllMenus();
        }

        private void SetComputerVsComputer()
        {
            GameMode = "ComputerVsComputer";
            IsBotSettingsEnabled = true;
            GameStatus = "Режим: Компьютер vs Компьютер";
            IsGameActive = true;

            PlayerVsPlayerBackground = Brushes.Transparent;
            PlayerVsComputerBackground = Brushes.Transparent;
            ComputerVsComputerBackground = Brushes.LightGreen;

            CloseAllMenus();
        }

        // Метод для переключения меню с закрытием при повторном нажатии
        private void ToggleMenu(string menuName)
        {
            // Сначала сбросим все фоны вкладок
            ResetTabBackgrounds();

            // Если кликаем по уже открытому меню - закрываем его
            if (_activeMenu == menuName)
            {
                CloseAllMenus();
                _activeMenu = string.Empty;
                return;
            }

            // Иначе закрываем все и открываем новое
            CloseAllMenus();
            _activeMenu = menuName;

            // Устанавливаем фон активной вкладки
            switch (menuName)
            {
                case nameof(IsGameMenuVisible):
                    IsGameMenuVisible = true;
                    GameTabBackground = Brushes.LightBlue;
                    break;
                case nameof(IsSettingsMenuVisible):
                    IsSettingsMenuVisible = true;
                    SettingsTabBackground = Brushes.LightBlue;
                    break;
                case nameof(IsModeMenuVisible):
                    IsModeMenuVisible = true;
                    ModeTabBackground = Brushes.LightBlue;
                    break;
                case nameof(IsAppearanceMenuVisible):
                    IsAppearanceMenuVisible = true;
                    AppearanceTabBackground = Brushes.LightBlue;
                    break;
                case nameof(IsStatsMenuVisible):
                    IsStatsMenuVisible = true;
                    StatsTabBackground = Brushes.LightBlue;
                    break;
            }

            GameStatus = $"Открыто меню: {GetMenuDisplayName(menuName)}";
        }

        // Метод для получения отображаемого имени меню
        private string GetMenuDisplayName(string menuName)
        {
            return menuName switch
            {
                nameof(IsGameMenuVisible) => "Главная",
                nameof(IsSettingsMenuVisible) => "Настройки",
                nameof(IsModeMenuVisible) => "Режим",
                nameof(IsAppearanceMenuVisible) => "Вид",
                nameof(IsStatsMenuVisible) => "Статистика",
                _ => menuName
            };
        }

        // Метод для сброса фонов вкладок
        private void ResetTabBackgrounds()
        {
            GameTabBackground = Brushes.Transparent;
            SettingsTabBackground = Brushes.Transparent;
            ModeTabBackground = Brushes.Transparent;
            AppearanceTabBackground = Brushes.Transparent;
            StatsTabBackground = Brushes.Transparent;
        }

        private void NewGame()
        {
            SaveBoardState();
            InitializeBoard();
            _isMultipleCapture = false;
            _currentCaptureCell = null;
            _capturedCellsInCurrentMove.Clear();
            GameStatus = "Новая игра началась! Ходят белые.";
            CurrentPlayer = "Белые";
            CurrentPlayerColor = "Белые шашки";
            SelectedCell = null;
            ClearPossibleMoves();
            CheckMandatoryCaptures();
            IsGameActive = true;
            ShowGameResult = false;
            CloseAllMenus();
        }

        private void UndoMove()
        {
            if (_moveHistory.Count > 0)
            {
                var previousState = _moveHistory.Pop();
                RestoreBoardState(previousState);

                _isMultipleCapture = false;
                _currentCaptureCell = null;
                _capturedCellsInCurrentMove.Clear();

                SwitchPlayer();
                ClearPossibleMoves();
                CheckMandatoryCaptures();

                GameStatus = "Ход отменен. Ходят " + CurrentPlayer;
            }
            else
            {
                GameStatus = "Нет ходов для отмены";
            }
            CloseAllMenus();
        }

        private void RedoMove()
        {
            GameStatus = "Ход повторен. Ходят " + CurrentPlayer;
            CloseAllMenus();
        }

        private void ResetGame()
        {
            NewGame();
        }

        // Измененный метод EndGame - теперь показывает окно результата при сдаче
        private void EndGame()
        {
            IsGameActive = false;
            _isMultipleCapture = false;
            _currentCaptureCell = null;
            _capturedCellsInCurrentMove.Clear();

            // Определяем, кто сдался (противоположный текущему игроку)
            string winner = CurrentPlayer == "Белые" ? "Черные" : "Белые";
            string loser = CurrentPlayer;

            // Показываем окно с результатом сдачи
            ShowGameResultMessage($"ИГРОК {loser} СДАЛСЯ!\nПОБЕДА {winner}!",
                winner == "Белые" ? Brushes.White : Brushes.Black);

            CloseAllMenus();
        }

        private void PauseGame()
        {
            IsGameActive = !IsGameActive;
            GameStatus = IsGameActive ? "Игра продолжена." : "Игра на паузе.";
            CloseAllMenus();
        }

        private void SaveGame()
        {
            GameStatus = "Игра сохранена.";
            CloseAllMenus();
        }

        private void LoadGame()
        {
            GameStatus = "Игра загружена.";
            CloseAllMenus();
        }

        private void ExitGame()
        {
            // При выходе через меню "Ничья" - засчитываем ничью
            IsGameActive = false;
            ShowGameResultMessage("НИЧЬЯ!\nИгра завершена по соглашению игроков.", Brushes.Gold);
            CloseAllMenus();
        }

        private void ToggleSound()
        {
            IsSoundEnabled = !IsSoundEnabled;
            SoundButtonBackground = IsSoundEnabled ? Brushes.LightGreen : Brushes.Transparent;
            GameStatus = IsSoundEnabled ? "Звук включен" : "Звук выключен";
            CloseAllMenus();
        }

        private void ToggleHints()
        {
            AreHintsEnabled = !AreHintsEnabled;
            HintsButtonBackground = AreHintsEnabled ? Brushes.LightGreen : Brushes.Transparent;
            GameStatus = AreHintsEnabled ? "Подсказки включены" : "Подсказки выключены";
            CloseAllMenus();
        }

        private void ToggleTimer()
        {
            IsTimerEnabled = !IsTimerEnabled;
            TimerButtonBackground = IsTimerEnabled ? Brushes.LightGreen : Brushes.Transparent;
            GameStatus = IsTimerEnabled ? "Таймер включен" : "Таймер выключен";
            CloseAllMenus();
        }

        private void ToggleDifficulty()
        {
            GameStatus = "Сложность изменена";
            CloseAllMenus();
        }

        private void ToggleBoardSize()
        {
            GameStatus = "Размер доски изменен";
            CloseAllMenus();
        }

        private void SetClassicColors()
        {
            GameStatus = "Установлены классические цвета";
            ClassicColorsBackground = Brushes.LightGreen;
            RedColorsBackground = Brushes.Transparent;
            BlueColorsBackground = Brushes.Transparent;
            CloseAllMenus();
        }

        private void SetRedColors()
        {
            GameStatus = "Установлены красные цвета";
            ClassicColorsBackground = Brushes.Transparent;
            RedColorsBackground = Brushes.LightGreen;
            BlueColorsBackground = Brushes.Transparent;
            CloseAllMenus();
        }

        private void SetBlueColors()
        {
            GameStatus = "Установлены синие цвета";
            ClassicColorsBackground = Brushes.Transparent;
            RedColorsBackground = Brushes.Transparent;
            BlueColorsBackground = Brushes.LightGreen;
            CloseAllMenus();
        }

        private void SetClassicBoard()
        {
            BoardBackground = Brushes.BurlyWood;
            GameStatus = "Установлена классическая доска";
            ClassicBoardBackground = Brushes.LightGreen;
            GreenBoardBackground = Brushes.Transparent;
            GrayBoardBackground = Brushes.Transparent;
            CloseAllMenus();
        }

        private void SetGreenBoard()
        {
            BoardBackground = Brushes.DarkGreen;
            GameStatus = "Установлена зеленая доска";
            ClassicBoardBackground = Brushes.Transparent;
            GreenBoardBackground = Brushes.LightGreen;
            GrayBoardBackground = Brushes.Transparent;
            CloseAllMenus();
        }

        private void SetGrayBoard()
        {
            BoardBackground = Brushes.Gray;
            GameStatus = "Установлена серая доска";
            ClassicBoardBackground = Brushes.Transparent;
            GreenBoardBackground = Brushes.Transparent;
            GrayBoardBackground = Brushes.LightGreen;
            CloseAllMenus();
        }

        private void ShowGameHistory()
        {
            GameStatus = "Показана история игр";
            CloseAllMenus();
        }

        private void ResetStats()
        {
            GamesPlayed = "Игр сыграно: 0";
            WhiteWins = "Побед белых: 0";
            BlackWins = "Побед черных: 0";
            GameStatus = "Статистика сброшена";
            CloseAllMenus();
        }

        private void ExportStats()
        {
            GameStatus = "Статистика экспортирована";
            CloseAllMenus();
        }

        private void ShowRules()
        {
            GameStatus = "Правила: 1) Обязательное взятие. 2) Можно бить назад. 3) Можно бить несколько шашек подряд. 4) После взятия можно продолжить бить.";
            CloseAllMenus();
        }

        private void ShowAbout()
        {
            GameStatus = "Шашки v4.0 - Шашки могут бить назад и бить несколько шашек";
            CloseAllMenus();
        }

        // Закрыть окно с результатом игры
        private void CloseGameResult()
        {
            ShowGameResult = false;
            NewGame();
        }

        // Обновленный метод CloseAllMenus с сбросом активной вкладки
        private void CloseAllMenus()
        {
            IsGameMenuVisible = false;
            IsSettingsMenuVisible = false;
            IsModeMenuVisible = false;
            IsAppearanceMenuVisible = false;
            IsStatsMenuVisible = false;
            ResetTabBackgrounds();
            _activeMenu = string.Empty;
        }

        // Основной метод для обработки кликов
        public void OnCellClick(CheckerCell clickedCell)
        {
            if (!IsGameActive) return;

            // Если находимся в режиме множественного взятия
            if (_isMultipleCapture && _currentCaptureCell != null)
            {
                // Можно кликнуть только на возможный ход для текущей шашки
                if (clickedCell.IsPossibleMove)
                {
                    TryMakeMove(_currentCaptureCell, clickedCell);
                }
                else
                {
                    // Клик не на возможный ход - снимаем выделение
                    GameStatus = "Выберите направление для продолжения взятия.";
                }
                return;
            }

            // Обычная логика
            // Если кликнули на шашку текущего игрока
            if (clickedCell.HasChecker &&
                ((CurrentPlayer == "Белые" && clickedCell.CheckerFill == Brushes.White) ||
                 (CurrentPlayer == "Черные" && clickedCell.CheckerFill == Brushes.Black)))
            {
                // Проверяем обязательное взятие
                if (_cellsWithMandatoryCapture.Count > 0 && !_cellsWithMandatoryCapture.Contains(clickedCell))
                {
                    GameStatus = "Обязательное взятие! Выберите шашку, которая может бить.";
                    return;
                }

                // Выделяем шашку
                SelectCell(clickedCell);
                GameStatus = $"Выбрана шашка [{clickedCell.Row},{clickedCell.Column}]";
            }
            // Если кликнули на возможный ход
            else if (clickedCell.IsPossibleMove && SelectedCell != null)
            {
                // Делаем ход
                bool moveSuccessful = TryMakeMove(SelectedCell, clickedCell);

                if (!moveSuccessful)
                {
                    ClearSelection();
                }
            }
            // Любой другой клик - снимаем выделение
            else
            {
                ClearSelection();
                GameStatus = $"Клетка [{clickedCell.Row},{clickedCell.Column}]. Выберите свою шашку.";
            }
        }

        // Выделить клетку
        private void SelectCell(CheckerCell cell)
        {
            // Снимаем выделение с предыдущей
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
            }

            // Выделяем новую
            cell.IsSelected = true;
            SelectedCell = cell;

            // Показываем возможные ходы
            ShowPossibleMoves(cell);
        }

        // Метод для очистки выделения
        public void ClearSelection()
        {
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
                SelectedCell = null;
            }
            ClearPossibleMoves();
            _isMultipleCapture = false;
            _currentCaptureCell = null;
            _capturedCellsInCurrentMove.Clear();
        }

        // Основной метод для выполнения хода
        public bool TryMakeMove(CheckerCell fromCell, CheckerCell toCell)
        {
            if (!IsGameActive || fromCell == null || toCell == null)
            {
                GameStatus = "Игра не активна";
                return false;
            }

            // Проверяем, что fromCell содержит шашку текущего игрока
            if (!fromCell.HasChecker)
            {
                GameStatus = "На этой клетке нет шашки!";
                return false;
            }

            // Проверяем, что это шашка текущего игрока
            if ((CurrentPlayer == "Белые" && fromCell.CheckerFill != Brushes.White) ||
                (CurrentPlayer == "Черные" && fromCell.CheckerFill != Brushes.Black))
            {
                GameStatus = "Не ваша шашка!";
                return false;
            }

            // Проверяем, что toCell свободна
            if (toCell.HasChecker)
            {
                GameStatus = "Клетка занята!";
                return false;
            }

            // Проверяем, что клетка черная (в шашках ходят только по черным клеткам)
            if ((toCell.Row + toCell.Column) % 2 == 0)
            {
                GameStatus = "Шашки ходят только по черным клеткам!";
                return false;
            }

            // Вычисляем разницу в координатах
            int rowDiff = toCell.Row - fromCell.Row;
            int colDiff = Math.Abs(toCell.Column - fromCell.Column);

            // Проверяем, что ход по диагонали
            if (Math.Abs(rowDiff) != colDiff)
            {
                GameStatus = "Шашки ходят только по диагонали!";
                return false;
            }

            // Проверяем расстояние
            int distance = Math.Abs(rowDiff);
            if (distance == 0)
            {
                GameStatus = "Нельзя оставаться на месте!";
                return false;
            }

            // Для обычной шашки
            if (!fromCell.IsKing)
            {
                // Проверяем расстояние
                if (distance == 1)
                {
                    // Простой ход на 1 клетку
                    return PerformSimpleMove(fromCell, toCell);
                }
                else if (distance == 2)
                {
                    // Взятие на 2 клетки (может бить назад)
                    return PerformCaptureMove(fromCell, toCell);
                }
                else
                {
                    GameStatus = "Обычная шашка может ходить только на 1 или 2 клетки!";
                    return false;
                }
            }
            else
            {
                // Для дамки
                return PerformKingMove(fromCell, toCell);
            }
        }

        // Простой ход на 1 клетку
        private bool PerformSimpleMove(CheckerCell fromCell, CheckerCell toCell)
        {
            // Проверяем, что нет обязательных взятий
            if (_cellsWithMandatoryCapture.Count > 0 && !_isMultipleCapture)
            {
                GameStatus = "Обязательное взятие! Нельзя сделать простой ход.";
                return false;
            }

            // Проверяем направление движения для обычной шашки
            if (!fromCell.IsKing)
            {
                // Для белых шашек - ход только вперед (на уменьшение row)
                // Для черных шашек - ход только вперед (на увеличение row)
                int rowDiff = toCell.Row - fromCell.Row;
                if ((fromCell.CheckerFill == Brushes.White && rowDiff > 0) ||
                    (fromCell.CheckerFill == Brushes.Black && rowDiff < 0))
                {
                    GameStatus = "Обычная шашка не может ходить назад!";
                    return false;
                }
            }

            // Сохраняем состояние до хода
            SaveBoardState();

            // Перемещаем шашку
            toCell.HasChecker = true;
            toCell.CheckerFill = fromCell.CheckerFill;
            toCell.IsKing = fromCell.IsKing;

            // Очищаем исходную клетку
            fromCell.HasChecker = false;
            fromCell.CheckerFill = Brushes.Transparent;
            fromCell.IsKing = false;
            fromCell.IsSelected = false;

            // Проверяем превращение в дамку
            CheckForKing(toCell);

            // Завершаем ход
            CompleteMove();
            GameStatus = "Ход сделан";

            return true;
        }

        // Взятие на 2 клетки - может сбить только одну шашку
        private bool PerformCaptureMove(CheckerCell fromCell, CheckerCell toCell)
        {
            // Находим клетку посередине
            int middleRow = (fromCell.Row + toCell.Row) / 2;
            int middleCol = (fromCell.Column + toCell.Column) / 2;

            var middleCell = GetCell(middleRow, middleCol);

            // Проверяем, что в середине вражеская шашка
            if (middleCell == null || !middleCell.HasChecker ||
                middleCell.CheckerFill == fromCell.CheckerFill)
            {
                GameStatus = "Невозможное взятие!";
                return false;
            }

            // Сохраняем состояние до хода
            SaveBoardState();

            // Убираем сбитую шашку
            middleCell.HasChecker = false;
            middleCell.CheckerFill = Brushes.Transparent;
            middleCell.IsKing = false;

            // Добавляем в список сбитых шашек за текущий ход
            _capturedCellsInCurrentMove.Add(middleCell);

            // Перемещаем свою шашку
            toCell.HasChecker = true;
            toCell.CheckerFill = fromCell.CheckerFill;
            toCell.IsKing = fromCell.IsKing;

            // Очищаем исходную клетку
            fromCell.HasChecker = false;
            fromCell.CheckerFill = Brushes.Transparent;
            fromCell.IsKing = false;
            fromCell.IsSelected = false;

            // Проверяем превращение в дамку
            CheckForKing(toCell);

            // Проверяем, можно ли продолжить бить с новой позиции
            if (CanJumpAgain(toCell, true))
            {
                // Включаем режим множественного взятия
                _isMultipleCapture = true;
                _currentCaptureCell = toCell;

                // Оставляем шашку выделенной для продолжения взятия
                toCell.IsSelected = true;
                SelectedCell = toCell;
                ShowPossibleMoves(toCell);

                GameStatus = $"Сбита шашка! Можно сбить еще. Всего сбито: {_capturedCellsInCurrentMove.Count}";
            }
            else
            {
                // Завершаем ход
                CompleteMove();
                GameStatus = $"Шашка противника сбита! Всего сбито за ход: {_capturedCellsInCurrentMove.Count}";
            }

            return true;
        }

        // Ход дамки с возможностью бить несколько шашек за один ход
        private bool PerformKingMove(CheckerCell fromCell, CheckerCell toCell)
        {
            int rowDiff = Math.Abs(toCell.Row - fromCell.Row);
            int colDiff = Math.Abs(toCell.Column - fromCell.Column);

            if (rowDiff != colDiff)
            {
                GameStatus = "Дамка ходит только по диагонали!";
                return false;
            }

            // Определяем направление движения
            int rowStep = toCell.Row > fromCell.Row ? 1 : -1;
            int colStep = toCell.Column > fromCell.Column ? 1 : -1;

            List<CheckerCell> capturedCells = new List<CheckerCell>();
            bool foundEnemy = false;
            bool canMove = true;

            // Проверяем путь на наличие шашек
            for (int i = 1; i < rowDiff; i++)
            {
                int checkRow = fromCell.Row + i * rowStep;
                int checkCol = fromCell.Column + i * colStep;

                var checkCell = GetCell(checkRow, checkCol);
                if (checkCell == null) continue;

                if (checkCell.HasChecker)
                {
                    if (checkCell.CheckerFill == fromCell.CheckerFill)
                    {
                        // Своя шашка на пути - нельзя ходить
                        canMove = false;
                        break;
                    }
                    else
                    {
                        // Вражеская шашка
                        capturedCells.Add(checkCell);
                        foundEnemy = true;
                    }
                }
            }

            if (!canMove)
            {
                GameStatus = "Нельзя перепрыгивать через свои шашки!";
                return false;
            }

            if (foundEnemy)
            {
                // Дамка бьет вражеские шашки
                return PerformMultipleKingCapture(fromCell, toCell, capturedCells);
            }
            else
            {
                // Простой ход дамки без взятия
                // Проверяем, что нет обязательных взятий
                if (_cellsWithMandatoryCapture.Count > 0 && !_isMultipleCapture)
                {
                    GameStatus = "Обязательное взятие! Нельзя сделать простой ход.";
                    return false;
                }

                // Сохраняем состояние до хода
                SaveBoardState();

                // Перемещаем дамку
                toCell.HasChecker = true;
                toCell.CheckerFill = fromCell.CheckerFill;
                toCell.IsKing = true;

                // Очищаем исходную клетку
                fromCell.HasChecker = false;
                fromCell.CheckerFill = Brushes.Transparent;
                fromCell.IsKing = false;
                fromCell.IsSelected = false;

                // Завершаем ход
                CompleteMove();
                GameStatus = "Дамка сходила";
                return true;
            }
        }

        // Множественное взятие дамкой (несколько шашек за один ход)
        private bool PerformMultipleKingCapture(CheckerCell fromCell, CheckerCell toCell, List<CheckerCell> capturedCells)
        {
            // Сохраняем состояние до хода
            SaveBoardState();

            // Убираем все сбитые шашки
            foreach (var capturedCell in capturedCells)
            {
                capturedCell.HasChecker = false;
                capturedCell.CheckerFill = Brushes.Transparent;
                capturedCell.IsKing = false;

                // Добавляем в список сбитых шашек за текущий ход
                _capturedCellsInCurrentMove.Add(capturedCell);
            }

            // Перемещаем дамку
            toCell.HasChecker = true;
            toCell.CheckerFill = fromCell.CheckerFill;
            toCell.IsKing = true;

            // Очищаем исходную клетку
            fromCell.HasChecker = false;
            fromCell.CheckerFill = Brushes.Transparent;
            fromCell.IsKing = false;
            fromCell.IsSelected = false;

            // Проверяем, можно ли продолжить бить с новой позиции
            if (CanJumpAgain(toCell, false))
            {
                // Включаем режим множественного взятия
                _isMultipleCapture = true;
                _currentCaptureCell = toCell;

                // Оставляем дамку выделенной для продолжения взятия
                toCell.IsSelected = true;
                SelectedCell = toCell;
                ShowPossibleMoves(toCell);
                GameStatus = $"Дамка сбила {capturedCells.Count} шашек! Можно сбить еще. Всего сбито: {_capturedCellsInCurrentMove.Count}";
            }
            else
            {
                // Завершаем ход
                CompleteMove();
                GameStatus = $"Дамка сбила {capturedCells.Count} шашек за один ход! Всего сбито: {_capturedCellsInCurrentMove.Count}";
            }

            return true;
        }

        // Проверяет, может ли шашка продолжить бить
        private bool CanJumpAgain(CheckerCell cell, bool isSimpleChecker)
        {
            // Проверяем все направления для взятия
            int[] directions = { -1, 1 };

            foreach (var rowDir in directions)
            {
                foreach (var colDir in directions)
                {
                    // Для дамки проверяем все расстояния
                    if (!isSimpleChecker && cell.IsKing)
                    {
                        for (int distance = 2; distance < BoardSize; distance++)
                        {
                            int jumpRow = cell.Row + distance * rowDir;
                            int jumpCol = cell.Column + distance * colDir;

                            if (CanKingCapture(cell, jumpRow, jumpCol, rowDir, colDir))
                            {
                                return true;
                            }
                        }
                    }
                    // Для обычной шашки - только на 2 клетки
                    else if (isSimpleChecker && !cell.IsKing)
                    {
                        int jumpRow = cell.Row + 2 * rowDir;
                        int jumpCol = cell.Column + 2 * colDir;
                        int middleRow = cell.Row + rowDir;
                        int middleCol = cell.Column + colDir;

                        if (CanCapture(cell, jumpRow, jumpCol, middleRow, middleCol))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // Проверка возможности взятия для дамки
        private bool CanKingCapture(CheckerCell fromCell, int toRow, int toCol, int rowDir, int colDir)
        {
            if (!IsValidCell(toRow, toCol)) return false;

            var targetCell = GetCell(toRow, toCol);
            if (targetCell.HasChecker) return false; // Целевая клетка должна быть свободна

            // Проверяем черную клетку
            if ((toRow + toCol) % 2 == 0) return false;

            int distance = Math.Abs(toRow - fromCell.Row);
            bool foundEnemy = false;

            for (int i = 1; i < distance; i++)
            {
                int checkRow = fromCell.Row + i * rowDir;
                int checkCol = fromCell.Column + i * colDir;

                var checkCell = GetCell(checkRow, checkCol);
                if (checkCell == null) continue;

                if (checkCell.HasChecker)
                {
                    if (checkCell.CheckerFill == fromCell.CheckerFill)
                    {
                        return false; // Своя шашка на пути
                    }
                    else
                    {
                        if (foundEnemy) return false; // Уже нашли вражескую шашку
                        foundEnemy = true;
                    }
                }
            }

            return foundEnemy; // Должна быть хотя бы одна вражеская шашка на пути
        }

        // Проверка превращения в дамку
        private void CheckForKing(CheckerCell cell)
        {
            if (!cell.IsKing &&
                ((cell.CheckerFill == Brushes.White && cell.Row == 0) ||
                 (cell.CheckerFill == Brushes.Black && cell.Row == BoardSize - 1)))
            {
                cell.IsKing = true;
                GameStatus = "Шашка стала дамкой!";

                // Если шашка стала дамкой во время множественного взятия,
                // проверяем может ли она продолжить бить как дамка
                if (_isMultipleCapture && _currentCaptureCell == cell)
                {
                    ClearPossibleMoves();
                    ShowPossibleMoves(cell);
                }
            }
        }

        // Завершение хода
        private void CompleteMove()
        {
            if (SelectedCell != null)
            {
                SelectedCell.IsSelected = false;
                SelectedCell = null;
            }

            // Обновляем счет после завершения хода
            UpdateScores();

            _isMultipleCapture = false;
            _currentCaptureCell = null;
            _capturedCellsInCurrentMove.Clear();

            ClearPossibleMoves();
            SwitchPlayer();
            CheckMandatoryCaptures();
        }

        // Показать возможные ходы
        private void ShowPossibleMoves(CheckerCell cell)
        {
            ClearPossibleMoves();

            // Если находимся в режиме множественного взятия, показываем только взятия
            if (_isMultipleCapture && _currentCaptureCell != null)
            {
                ShowCaptureMoves(cell);
                return;
            }

            // Если есть обязательные взятия для других шашек
            if (_cellsWithMandatoryCapture.Count > 0)
            {
                // Проверяем, что эта шашка может бить
                if (_cellsWithMandatoryCapture.Contains(cell))
                {
                    ShowCaptureMoves(cell);
                }
                else
                {
                    GameStatus = "Обязательное взятие! Выберите шашку, которая может бить.";
                }
                return;
            }

            // Проверяем, может ли эта шашка делать взятие
            bool canCapture = HasAnyCaptureMove(cell);

            if (canCapture)
            {
                // Показываем только взятия
                ShowCaptureMoves(cell);
                GameStatus = "Можно сделать взятие!";
                return;
            }

            // Показываем все возможные ходы (простые)
            ShowAllPossibleMoves(cell);
        }

        // Показать все возможные ходы
        private void ShowAllPossibleMoves(CheckerCell cell)
        {
            if (cell.IsKing)
            {
                ShowKingMoves(cell);
            }
            else
            {
                ShowSimpleCheckerMoves(cell);
            }
        }

        // Показать ходы обычной шашки (только вперед)
        private void ShowSimpleCheckerMoves(CheckerCell cell)
        {
            // Определяем направление движения
            int rowDir = cell.CheckerFill == Brushes.White ? -1 : 1;

            // Простые ходы на 1 клетку вперед
            CheckMove(cell, cell.Row + rowDir, cell.Column - 1); // Влево вперед
            CheckMove(cell, cell.Row + rowDir, cell.Column + 1); // Вправо вперед
        }

        // Показать ходы дамки
        private void ShowKingMoves(CheckerCell cell)
        {
            int[] directions = { -1, 1 };

            foreach (var rowDir in directions)
            {
                foreach (var colDir in directions)
                {
                    // Проверяем все клетки по диагонали
                    for (int distance = 1; distance < BoardSize; distance++)
                    {
                        int newRow = cell.Row + distance * rowDir;
                        int newCol = cell.Column + distance * colDir;

                        if (!IsValidCell(newRow, newCol)) break;

                        var targetCell = GetCell(newRow, newCol);

                        // Проверяем черную клетку
                        if ((newRow + newCol) % 2 == 0) break;

                        if (targetCell.HasChecker)
                        {
                            break; // Нельзя ходить на занятую клетку
                        }
                        else
                        {
                            // Свободная клетка
                            targetCell.IsPossibleMove = true;
                        }
                    }
                }
            }
        }

        // Показать ходы со взятием
        private void ShowCaptureMoves(CheckerCell cell)
        {
            if (cell.IsKing)
            {
                ShowKingCaptureMoves(cell);
            }
            else
            {
                ShowSimpleCaptureMoves(cell);
            }
        }

        // Показать взятия обычной шашки (вперед и назад)
        private void ShowSimpleCaptureMoves(CheckerCell cell)
        {
            int[] directions = { -1, 1 }; // И вперед, и назад для взятия

            foreach (var rowDir in directions)
            {
                // Взятие влево
                CheckCaptureMove(cell, cell.Row + rowDir * 2, cell.Column - 2,
                               cell.Row + rowDir, cell.Column - 1);

                // Взятие вправо
                CheckCaptureMove(cell, cell.Row + rowDir * 2, cell.Column + 2,
                               cell.Row + rowDir, cell.Column + 1);
            }
        }

        // Показать взятия дамки
        private void ShowKingCaptureMoves(CheckerCell cell)
        {
            int[] directions = { -1, 1 };

            foreach (var rowDir in directions)
            {
                foreach (var colDir in directions)
                {
                    for (int distance = 2; distance < BoardSize; distance++)
                    {
                        int jumpRow = cell.Row + distance * rowDir;
                        int jumpCol = cell.Column + distance * colDir;

                        if (!IsValidCell(jumpRow, jumpCol)) break;

                        var targetCell = GetCell(jumpRow, jumpCol);

                        // Проверяем черную клетку
                        if ((jumpRow + jumpCol) % 2 == 0) break;

                        if (!targetCell.HasChecker)
                        {
                            // Проверяем путь на наличие вражеской шашки
                            bool foundEnemy = false;
                            bool foundOwn = false;

                            for (int i = 1; i < distance; i++)
                            {
                                int checkRow = cell.Row + i * rowDir;
                                int checkCol = cell.Column + i * colDir;

                                var checkCell = GetCell(checkRow, checkCol);
                                if (checkCell == null) continue;

                                if (checkCell.HasChecker)
                                {
                                    if (checkCell.CheckerFill == cell.CheckerFill)
                                    {
                                        foundOwn = true;
                                        break;
                                    }
                                    else
                                    {
                                        if (foundEnemy)
                                        {
                                            foundOwn = true; // Более одной вражеской шашки
                                            break;
                                        }
                                        foundEnemy = true;
                                    }
                                }
                            }

                            if (foundEnemy && !foundOwn)
                            {
                                targetCell.IsPossibleMove = true;
                            }
                        }
                    }
                }
            }
        }

        // Проверить и отметить ход
        private void CheckMove(CheckerCell fromCell, int toRow, int toCol)
        {
            if (!IsValidCell(toRow, toCol)) return;

            var targetCell = GetCell(toRow, toCol);

            // Проверяем черную клетку
            if ((toRow + toCol) % 2 == 0) return;

            if (!targetCell.HasChecker)
            {
                targetCell.IsPossibleMove = true;
            }
        }

        // Проверить и отметить взятие
        private void CheckCaptureMove(CheckerCell fromCell, int toRow, int toCol, int middleRow, int middleCol)
        {
            if (!IsValidCell(toRow, toCol) || !IsValidCell(middleRow, middleCol)) return;

            var targetCell = GetCell(toRow, toCol);
            var middleCell = GetCell(middleRow, middleCol);

            if (!targetCell.HasChecker &&
                middleCell.HasChecker &&
                middleCell.CheckerFill != fromCell.CheckerFill &&
                (toRow + toCol) % 2 == 1)
            {
                targetCell.IsPossibleMove = true;
            }
        }

        // Проверить обязательные взятия
        private void CheckMandatoryCaptures()
        {
            ClearMandatoryCaptures();
            _cellsWithMandatoryCapture.Clear();

            var currentPlayerColor = CurrentPlayer == "Белые" ? Brushes.White : Brushes.Black;
            bool hasCaptureMoves = false;

            // Проверяем все шашки текущего игрока
            foreach (var cell in BoardCells)
            {
                if (cell.HasChecker && cell.CheckerFill == currentPlayerColor)
                {
                    if (HasAnyCaptureMove(cell))
                    {
                        cell.IsMandatoryCapture = true;
                        _cellsWithMandatoryCapture.Add(cell);
                        hasCaptureMoves = true;
                    }
                }
            }

            if (hasCaptureMoves)
            {
                GameStatus = "Обязательное взятие! Выберите шашку, которая может бить.";
            }
        }

        // Проверить, есть ли у шашки взятия
        private bool HasAnyCaptureMove(CheckerCell cell)
        {
            if (cell.IsKing)
            {
                return HasKingCaptureMoves(cell);
            }
            else
            {
                return HasSimpleCaptureMoves(cell);
            }
        }

        // Проверить взятия обычной шашки (вперед и назад)
        private bool HasSimpleCaptureMoves(CheckerCell cell)
        {
            int[] directions = { -1, 1 }; // И вперед, и назад для взятия

            foreach (var rowDir in directions)
            {
                if (CanCapture(cell, cell.Row + rowDir * 2, cell.Column - 2,
                            cell.Row + rowDir, cell.Column - 1) ||
                    CanCapture(cell, cell.Row + rowDir * 2, cell.Column + 2,
                            cell.Row + rowDir, cell.Column + 1))
                {
                    return true;
                }
            }

            return false;
        }

        // Проверить взятия дамки
        private bool HasKingCaptureMoves(CheckerCell cell)
        {
            int[] directions = { -1, 1 };

            foreach (var rowDir in directions)
            {
                foreach (var colDir in directions)
                {
                    for (int distance = 2; distance < BoardSize; distance++)
                    {
                        int jumpRow = cell.Row + distance * rowDir;
                        int jumpCol = cell.Column + distance * colDir;

                        if (!IsValidCell(jumpRow, jumpCol)) break;

                        if (CanKingCapture(cell, jumpRow, jumpCol, rowDir, colDir))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        // Проверить возможность взятия
        private bool CanCapture(CheckerCell fromCell, int jumpRow, int jumpCol, int middleRow, int middleCol)
        {
            if (!IsValidCell(jumpRow, jumpCol) || !IsValidCell(middleRow, middleCol))
                return false;

            var targetCell = GetCell(jumpRow, jumpCol);
            var middleCell = GetCell(middleRow, middleCol);

            return !targetCell.HasChecker &&
                   middleCell.HasChecker &&
                   middleCell.CheckerFill != fromCell.CheckerFill &&
                   (jumpRow + jumpCol) % 2 == 1;
        }

        // Переключить игрока
        private void SwitchPlayer()
        {
            if (CurrentPlayer == "Белые")
            {
                CurrentPlayer = "Черные";
                CurrentPlayerColor = "Черные шашки";
            }
            else
            {
                CurrentPlayer = "Белые";
                CurrentPlayerColor = "Белые шашки";
            }

            GameStatus = $"Ход завершен. Ходят {CurrentPlayer}";
        }

        // Обновить счет
        private void UpdateScores()
        {
            int whiteCount = 0;
            int blackCount = 0;

            foreach (var cell in BoardCells)
            {
                if (cell.HasChecker)
                {
                    if (cell.CheckerFill == Brushes.White)
                        whiteCount++;
                    else
                        blackCount++;
                }
            }

            WhiteScore = $"Белые: {whiteCount}";
            BlackScore = $"Черные: {blackCount}";

            // Проверка конца игры
            if (whiteCount == 0 && blackCount == 0)
            {
                ShowGameResultMessage("НИЧЬЯ!\nНа доске не осталось шашек.", Brushes.Gold);
            }
            else if (whiteCount == 0)
            {
                ShowGameResultMessage("ПОБЕДА ЧЕРНЫХ!\nБелых шашек не осталось на доске.", Brushes.Black);
            }
            else if (blackCount == 0)
            {
                ShowGameResultMessage("ПОБЕДА БЕЛЫХ!\nЧерных шашек не осталось на доске.", Brushes.White);
            }
        }

        // Показать сообщение о результате игры
        private void ShowGameResultMessage(string message, IBrush background)
        {
            IsGameActive = false;
            GameResultMessage = message;
            GameResultBackground = background;
            ShowGameResult = true;

            // Обновляем статистику
            if (message.Contains("БЕЛЫХ"))
                UpdateStatistics(true);
            else if (message.Contains("ЧЕРНЫХ"))
                UpdateStatistics(false);
            else if (message.Contains("НИЧЬЯ"))
                UpdateStatistics("Ничья");
        }

        // Обновить статистику
        private void UpdateStatistics(bool whiteWon)
        {
            try
            {
                var gamesPlayed = int.Parse(GamesPlayed.Split(':')[1].Trim());
                var whiteWins = int.Parse(WhiteWins.Split(':')[1].Trim());
                var blackWins = int.Parse(BlackWins.Split(':')[1].Trim());

                gamesPlayed++;
                if (whiteWon)
                    whiteWins++;
                else
                    blackWins++;

                GamesPlayed = $"Игр сыграно: {gamesPlayed}";
                WhiteWins = $"Побед белых: {whiteWins}";
                BlackWins = $"Побед черных: {blackWins}";
            }
            catch
            {
                GamesPlayed = "Игр сыграно: 1";
                if (whiteWon)
                {
                    WhiteWins = "Побед белых: 1";
                    BlackWins = "Побед черных: 0";
                }
                else
                {
                    WhiteWins = "Побед белых: 0";
                    BlackWins = "Побед черных: 1";
                }
            }
        }

        // Обновить статистику для ничьей
        private void UpdateStatistics(string result)
        {
            try
            {
                var gamesPlayed = int.Parse(GamesPlayed.Split(':')[1].Trim());
                gamesPlayed++;

                GamesPlayed = $"Игр сыграно: {gamesPlayed}";
            }
            catch
            {
                GamesPlayed = "Игр сыграно: 1";
            }
        }

        // Получить клетку по координатам
        public CheckerCell GetCell(int row, int column)
        {
            if (row < 0 || row >= BoardSize || column < 0 || column >= BoardSize)
                return null;

            return BoardCells[row * BoardSize + column];
        }

        // Проверить валидность клетки
        private bool IsValidCell(int row, int col)
        {
            return row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
        }

        // Очистить возможные ходы
        private void ClearPossibleMoves()
        {
            foreach (var cell in BoardCells)
            {
                cell.IsPossibleMove = false;
            }
        }

        // Очистить подсветку обязательных взятий
        private void ClearMandatoryCaptures()
        {
            foreach (var cell in BoardCells)
            {
                cell.IsMandatoryCapture = false;
            }
        }

        // Сохранить состояние доски
        private void SaveBoardState()
        {
            var state = new List<CheckerCell>();
            foreach (var cell in BoardCells)
            {
                state.Add(new CheckerCell
                {
                    Row = cell.Row,
                    Column = cell.Column,
                    Background = cell.Background,
                    HasChecker = cell.HasChecker,
                    CheckerFill = cell.CheckerFill,
                    IsKing = cell.IsKing,
                    IsSelected = cell.IsSelected,
                    IsPossibleMove = cell.IsPossibleMove,
                    IsMandatoryCapture = cell.IsMandatoryCapture
                });
            }
            _moveHistory.Push(state);
        }

        // Восстановить состояние доски
        private void RestoreBoardState(List<CheckerCell> state)
        {
            for (int i = 0; i < BoardCells.Count && i < state.Count; i++)
            {
                var source = state[i];
                var target = BoardCells[i];

                target.HasChecker = source.HasChecker;
                target.CheckerFill = source.CheckerFill;
                target.IsKing = source.IsKing;
                target.IsSelected = source.IsSelected;
                target.IsPossibleMove = source.IsPossibleMove;
                target.IsMandatoryCapture = source.IsMandatoryCapture;
            }
        }

        // Класс клетки внутри ViewModel
        public class CheckerCell : ReactiveObject
        {
            private IBrush _background;
            private IBrush _checkerFill;
            private bool _hasChecker;
            private bool _isKing;
            private bool _isSelected;
            private bool _isPossibleMove;
            private bool _isMandatoryCapture;

            public int Row { get; set; }
            public int Column { get; set; }

            public IBrush Background
            {
                get => _background;
                set => this.RaiseAndSetIfChanged(ref _background, value);
            }

            public IBrush CheckerFill
            {
                get => _checkerFill;
                set => this.RaiseAndSetIfChanged(ref _checkerFill, value);
            }

            public bool HasChecker
            {
                get => _hasChecker;
                set => this.RaiseAndSetIfChanged(ref _hasChecker, value);
            }

            public bool IsKing
            {
                get => _isKing;
                set => this.RaiseAndSetIfChanged(ref _isKing, value);
            }

            public bool IsSelected
            {
                get => _isSelected;
                set => this.RaiseAndSetIfChanged(ref _isSelected, value);
            }

            public bool IsPossibleMove
            {
                get => _isPossibleMove;
                set => this.RaiseAndSetIfChanged(ref _isPossibleMove, value);
            }

            public bool IsMandatoryCapture
            {
                get => _isMandatoryCapture;
                set => this.RaiseAndSetIfChanged(ref _isMandatoryCapture, value);
            }
        }
    }
}