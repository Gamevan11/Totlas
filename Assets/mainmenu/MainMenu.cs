using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject PlayGamePanel;

    public Animation PlayAnimation;
    public Animation OptionsAnimation;

    public Text PlayText;
    public Text OptionsText;

    bool play;
    bool options;

    // Список серверов
    public void onPlayGame()
    {
        if (!PlayAnimation.IsPlaying("playOpen") && !PlayAnimation.IsPlaying("playClose") && !options)
        {
            play = !play;

            if (play) // Открыть панель
            {
                PlayAnimation.Play("playOpen");
                PlayText.color = Color.white; // Меняем цвет текста
            }
            else if (!play) // Закрыть панель
            {
                PlayAnimation.Play("playClose");
                PlayText.color = new Color32(195, 195, 195, 255); // Меняем цвет текста
            }
        }
    }


    // // Настройки
    public void onOptions()
    {
        if (!PlayAnimation.IsPlaying("playOpen") && !PlayAnimation.IsPlaying("playClose") && !play)
        {
            options = !options;

            if (options) // Открыть панель
            {
                OptionsAnimation.Play("playOpen");
                OptionsText.color = Color.white; // Меняем цвет текста
            }
            else if (!options) // Закрыть панель
            {
                OptionsAnimation.Play("playClose");
                OptionsText.color = new Color32(195, 195, 195, 255); // Меняем цвет текста
            }
        }
    }

    public void YouTube()
    {
        Application.OpenURL("https://youtube.com/@totlassurvivalafternuclear4137");
    }

    public void Discjrd()
    {
        Application.OpenURL("https://discord.gg/j3QfkTKtnu");
    }

    public void Telegram()
    {
        Application.OpenURL("https://t.me/totlas_Gamevan");
    }

    // Выход из игры
    public void onQuit() => Application.Quit();
}
