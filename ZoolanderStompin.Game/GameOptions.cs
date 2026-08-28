using System.Text.Json;
using System.Text.Json.Serialization;

namespace ZoolanderStompin.Game;

public sealed class GameOptions
{
    public const string SectionName = "Game";

    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(allowIntegerValues: false) },
        WriteIndented = true,
    };

    public int DebounceMilliseconds { get; set; }

    public int PresentationsPerRound { get; set; }

    public int RoundCount { get; set; }

    public bool PreventConsecutiveRepeat { get; set; }

    public int InterTargetGapMilliseconds { get; set; }

    public int WinPercentThreshold { get; set; }

    public int SelectTimeoutSeconds { get; set; }

    public SelectTimeoutAction SelectTimeoutAction { get; set; }

    public int CountdownGetReadyMilliseconds { get; set; }

    public int CountdownGoMilliseconds { get; set; }

    public int IntermissionMilliseconds { get; set; }

    public int ResultsMilliseconds { get; set; }

    public int AttractLampCycleMilliseconds { get; set; }

    public bool FreePlay { get; set; }

    public int CoinsPerCredit { get; set; }

    public DifficultyOptions Easy { get; set; } = new();

    public DifficultyOptions Medium { get; set; } = new();

    public DifficultyOptions Hard { get; set; } = new();

    public PayoutOptions Payout { get; set; } = new();

    public DifficultyOptions For(Difficulty difficulty) => difficulty switch
    {
        Difficulty.Easy => Easy,
        Difficulty.Medium => Medium,
        Difficulty.Hard => Hard,
        _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null),
    };

    public static GameOptions CreateDefault()
    {
        var options = new GameOptions
        {
            DebounceMilliseconds = 30,
            PresentationsPerRound = 20,
            RoundCount = 2,
            PreventConsecutiveRepeat = true,
            InterTargetGapMilliseconds = 250,
            WinPercentThreshold = 60,
            SelectTimeoutSeconds = 30,
            SelectTimeoutAction = SelectTimeoutAction.AutoStartEasy,
            CountdownGetReadyMilliseconds = 1500,
            CountdownGoMilliseconds = 700,
            IntermissionMilliseconds = 2000,
            ResultsMilliseconds = 3000,
            AttractLampCycleMilliseconds = 400,
            FreePlay = false,
            CoinsPerCredit = 1,
            Easy = new DifficultyOptions
            {
                PadsInPlay = [1, 2, 3, 4],
                HitWindowMilliseconds = 1200,
            },
            Medium = new DifficultyOptions
            {
                PadsInPlay = [1, 2, 3, 4, 5, 6],
                HitWindowMilliseconds = 900,
            },
            Hard = new DifficultyOptions
            {
                PadsInPlay = [1, 2, 3, 4, 5, 6, 7],
                HitWindowMilliseconds = 650,
            },
            Payout = new PayoutOptions
            {
                Mode = PayoutMode.PercentageTable,
                FixedTickets = 0,
                Table =
                [
                    new PayoutBand { MinPercentInclusive = 0, MaxPercentInclusive = 19, Tickets = 0 },
                    new PayoutBand { MinPercentInclusive = 20, MaxPercentInclusive = 39, Tickets = 1 },
                    new PayoutBand { MinPercentInclusive = 40, MaxPercentInclusive = 59, Tickets = 2 },
                    new PayoutBand { MinPercentInclusive = 60, MaxPercentInclusive = 79, Tickets = 4 },
                    new PayoutBand { MinPercentInclusive = 80, MaxPercentInclusive = 99, Tickets = 6 },
                    new PayoutBand { MinPercentInclusive = 100, MaxPercentInclusive = 100, Tickets = 8 },
                ],
            },
        };

        options.EnsureValid();
        return options;
    }

    public static GameOptions FromJson(string json)
    {
        GameOptions options;
        try
        {
            options = JsonSerializer.Deserialize<GameOptions>(json, JsonOptions)
                ?? throw new GameConfigurationException("Game configuration JSON deserialized to null.");
        }
        catch (JsonException ex)
        {
            throw new GameConfigurationException($"Game configuration JSON is invalid: {ex.Message}");
        }

        options.EnsureValid();
        return options;
    }

    public void EnsureValid()
    {
        var errors = new List<string>();
        ValidateTiming(errors);
        ValidateDifficulty("Easy", Easy, errors);
        ValidateDifficulty("Medium", Medium, errors);
        ValidateDifficulty("Hard", Hard, errors);
        ValidatePayout(Payout, errors);

        if (errors.Count > 0)
        {
            throw new GameConfigurationException(string.Join(" ", errors));
        }
    }

    private void ValidateTiming(List<string> errors)
    {
        if (DebounceMilliseconds <= 0)
        {
            errors.Add("DebounceMilliseconds must be greater than 0.");
        }

        if (PresentationsPerRound is < 1 or > 99)
        {
            errors.Add("PresentationsPerRound must be between 1 and 99 (two-digit score display).");
        }

        if (RoundCount < 1)
        {
            errors.Add("RoundCount must be at least 1.");
        }

        if (InterTargetGapMilliseconds < 0)
        {
            errors.Add("InterTargetGapMilliseconds cannot be negative.");
        }

        if (WinPercentThreshold is < 0 or > 100)
        {
            errors.Add("WinPercentThreshold must be between 0 and 100.");
        }

        if (SelectTimeoutSeconds <= 0)
        {
            errors.Add("SelectTimeoutSeconds must be greater than 0.");
        }

        if (!Enum.IsDefined(SelectTimeoutAction))
        {
            errors.Add("SelectTimeoutAction is not a recognized value.");
        }

        if (CountdownGetReadyMilliseconds <= 0)
        {
            errors.Add("CountdownGetReadyMilliseconds must be greater than 0.");
        }

        if (CountdownGoMilliseconds <= 0)
        {
            errors.Add("CountdownGoMilliseconds must be greater than 0.");
        }

        if (IntermissionMilliseconds <= 0)
        {
            errors.Add("IntermissionMilliseconds must be greater than 0.");
        }

        if (ResultsMilliseconds <= 0)
        {
            errors.Add("ResultsMilliseconds must be greater than 0.");
        }

        if (AttractLampCycleMilliseconds <= 0)
        {
            errors.Add("AttractLampCycleMilliseconds must be greater than 0.");
        }

        if (CoinsPerCredit < 1)
        {
            errors.Add("CoinsPerCredit must be at least 1.");
        }
    }

    private static void ValidateDifficulty(string name, DifficultyOptions? difficulty, List<string> errors)
    {
        if (difficulty is null)
        {
            errors.Add($"{name} difficulty settings are required.");
            return;
        }

        if (difficulty.HitWindowMilliseconds <= 0)
        {
            errors.Add($"{name} HitWindowMilliseconds must be greater than 0.");
        }

        if (difficulty.PadsInPlay is null || difficulty.PadsInPlay.Length == 0)
        {
            errors.Add($"{name} must list at least one pad in play.");
            return;
        }

        var seen = new HashSet<int>();
        foreach (var pad in difficulty.PadsInPlay)
        {
            if (pad is < 1 or > FloorPad.Count)
            {
                errors.Add($"{name} pad {pad} is not between 1 and {FloorPad.Count}.");
            }
            else if (!seen.Add(pad))
            {
                errors.Add($"{name} lists pad {pad} more than once.");
            }
        }
    }

    private static void ValidatePayout(PayoutOptions? payout, List<string> errors)
    {
        if (payout is null)
        {
            errors.Add("Payout settings are required.");
            return;
        }

        if (!Enum.IsDefined(payout.Mode))
        {
            errors.Add("Payout mode is not a recognized value.");
        }

        if (payout.FixedTickets < 0)
        {
            errors.Add("FixedTickets cannot be negative.");
        }

        if (payout.Mode == PayoutMode.Fixed)
        {
            return;
        }

        if (payout.Table is null || payout.Table.Count == 0)
        {
            errors.Add("Percentage payout requires a non-empty table.");
            return;
        }

        var ordered = payout.Table.OrderBy(b => b.MinPercentInclusive).ToList();
        if (ordered[0].MinPercentInclusive != 0)
        {
            errors.Add("Payout table must start at 0%.");
        }

        if (ordered[^1].MaxPercentInclusive != 100)
        {
            errors.Add("Payout table must end at 100%.");
        }

        for (var i = 0; i < ordered.Count; i++)
        {
            var band = ordered[i];
            if (band.MinPercentInclusive > band.MaxPercentInclusive)
            {
                errors.Add($"Payout band {band.MinPercentInclusive}-{band.MaxPercentInclusive} has min greater than max.");
            }

            if (band.MinPercentInclusive < 0 || band.MaxPercentInclusive > 100)
            {
                errors.Add($"Payout band {band.MinPercentInclusive}-{band.MaxPercentInclusive} is outside 0-100.");
            }

            if (band.Tickets < 0)
            {
                errors.Add($"Payout band {band.MinPercentInclusive}-{band.MaxPercentInclusive} has negative tickets.");
            }

            if (i == 0)
            {
                continue;
            }

            var previous = ordered[i - 1];
            if (band.MinPercentInclusive != previous.MaxPercentInclusive + 1)
            {
                errors.Add(
                    $"Payout table has a gap or overlap between {previous.MaxPercentInclusive}% and {band.MinPercentInclusive}%.");
            }
        }
    }
}
