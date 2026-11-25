namespace FlappyBird;

public enum DifficultyLevel
{
    Easy,
    Normal,
    Hard
}

// Zorluk seviyesine göre oyun parametrelerini döndürür
public static class Difficulty
{
    public static float GetBaseSpeed(DifficultyLevel level) => level switch
    {
        DifficultyLevel.Easy   => 2.5f,
        DifficultyLevel.Normal => 3.5f,
        DifficultyLevel.Hard   => 5.0f,
        _                      => 3.5f
    };

    public static int GetGapSize(DifficultyLevel level) => level switch
    {
        DifficultyLevel.Easy   => 200,
        DifficultyLevel.Normal => 160,
        DifficultyLevel.Hard   => 120,
        _                      => 160
    };

    public static string GetLabel(DifficultyLevel level) => level switch
    {
        DifficultyLevel.Easy   => "Kolay",
        DifficultyLevel.Normal => "Normal",
        DifficultyLevel.Hard   => "Zor",
        _                      => "Normal"
    };
}
