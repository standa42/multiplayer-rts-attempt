#define IS_ANDROID

using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Common;
using Assets.Scripts.Game.Races;
using Assets.Scripts.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

/// <summary>
/// Controller of UI behaviour in the main menu
/// </summary>
public class MenuControl : MonoBehaviour
{
    // UI elements to be switched un/visible

    public GameObject SignInPanel;
    public GameObject NumberOfPlayersChoicePanel;
    public GameObject RaceChoicePanel;
    public GameObject PlayPanel;

    public Text SignInSuccessfulText;
    public Text SignInFailedText;

    public List<Button> RaceButtons;
    public Button UniversalRaceButton;
    public Button SecondRaceButton;
    public Button StartButton;
    public Button SignInButton;
    public Button CancelSignInButton;


    // Game input variables to be passed to game scene

    private NumberOfPlayersInGame numberOfPlayersInGame;
    private RaceEnum raceEnum;


    // Signing in variable
    private bool? signedInSuccesfully = null;


    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    // Callbacks binded to UI elements

    public void UniversalRaceClick()
    {
        ChooseRaceButtons(UniversalRaceButton, RaceEnum.Universal);
    }

    public void SecondRaceClick()
    {
        ChooseRaceButtons(SecondRaceButton,RaceEnum.Second);
    }

    public void FourPlayersClick()
    {
        numberOfPlayersInGame = NumberOfPlayersInGame.Four;
        TransitionFromNumberToRace();
    }

    public void TwoPlayersClick()
    {
        numberOfPlayersInGame = NumberOfPlayersInGame.Two;
        TransitionFromNumberToRace();
    }

    public void StartClick()
    {
        IntersceneData.MenuChoicesInstance = new MenuChoices(numberOfPlayersInGame, raceEnum);
        SceneManager.LoadScene("GameTrial");
    }

    public void PlayClick()
    {
#if IS_ANDROID
        bool isSignedIn = PlayGamesPlatform.Instance.localUser.authenticated;

        if (isSignedIn)
        {
            Log.LogMessage("Already signed -> goto numOfPlayers");
            TransitionFromPlayToNumberOfPlayers();
        }
        else
        {
            Log.LogMessage("Not signed -> goto signing");
            TransitionFromPlayToSignIn();
        }
#endif

#if !IS_ANDROID
        TransitionFromPlayToNumberOfPlayers();
#endif
    }

    public void SignInClick()
    {
        DisableSignInButtons();

        // success is useless - handled by coroutine
        bool? success = SignInToGooglePlayGames();
    }

    public void CancelSignInClick()
    {
        TransitionFromSignInToPlay(null);
    }

    public void SignOutClick()
    {
        PlayGamesPlatform.Instance.SignOut();
        signedInSuccesfully = null;

        Log.LogMessage("Signed out");
    }


    // Helper functions

    private void TransitionFromNumberToRace()
    {
        NumberOfPlayersChoicePanel.SetActive(false);
        RaceChoicePanel.SetActive(true);
    }

    /// <summary>
    /// Shows different texts according to un/successful connection to google game account
    /// </summary>
    /// <param name="success"></param>
    private void TransitionFromSignInToPlay(bool? success)
    {
        switch (success)
        {
            case true:
                StartCoroutine(DisplayTextForGivenTime(SignInSuccessfulText, 3f));
                break;

            case false:
                StartCoroutine(DisplayTextForGivenTime(SignInFailedText, 4f));
                break;

            case null:

                break;
        }

        SignInPanel.SetActive(false);
        SignInButton.interactable = true;
        CancelSignInButton.interactable = true;

        PlayPanel.SetActive(true);
    }

    private void TransitionFromPlayToSignIn()
    {
        PlayPanel.SetActive(false);
        SignInPanel.SetActive(true);
    }

    private void TransitionFromPlayToNumberOfPlayers()
    {
        PlayPanel.SetActive(false);
        NumberOfPlayersChoicePanel.SetActive(true);
    }

    private void DisableSignInButtons()
    {
        SignInButton.interactable = false;
        CancelSignInButton.interactable = false;
    }

    private void SetRaceButtonsInteractable()
    {
        foreach (var button in RaceButtons)
        {
            button.interactable = true;
        }
    }

    private bool? SignInToGooglePlayGames()
    {
        Log.LogMessage("Signing in started");
        if (PlayGamesPlatform.Instance.localUser.authenticated)
        {
            Log.LogMessage("already signed in");
            return true;
        }

        PlayGamesPlatform.Instance.Authenticate((bool success) => { signedInSuccesfully = success; });

        Log.LogMessage("Auth coroutine call");
        StartCoroutine("SigningInEnumerator");

        Log.LogMessage("signing in returned" + signedInSuccesfully);
        return signedInSuccesfully;
    }

    /// <summary>
    /// Makes selected button not interactable and sets its race as chosen
    /// </summary>
    /// <returns></returns>
    private void ChooseRaceButtons(Button btn, RaceEnum raceEnum)
    {
        SetRaceButtonsInteractable();
        btn.interactable = false;
        this.raceEnum = raceEnum;
        StartButton.interactable = true;
    }


    // UI Coroutines

    private IEnumerator DisplayTextForGivenTime(Text text, float timeInSeconds)
    {
        text.enabled = true;
        yield return new WaitForSeconds(timeInSeconds);
        text.enabled = false;
    }

    private IEnumerator SigningInEnumerator()
    {
        Log.LogMessage("auth coroutine called");

        while (signedInSuccesfully == null)
        {
            Log.LogMessage("auth waiting");
            yield return new WaitForSeconds(0.1f);
        }

        Log.LogMessage("auth complete: " + signedInSuccesfully);
        TransitionFromSignInToPlay(signedInSuccesfully);
    }


    // Test scenes buttons

    public void ConnectionTestClick()
    {
        SceneManager.LoadScene("ConnectionTest");
    }
}

