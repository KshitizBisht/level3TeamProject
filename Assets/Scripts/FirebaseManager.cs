using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

public class FirebaseManager : MonoBehaviour

{

    public static FirebaseManager instance;
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;
    public DatabaseReference DBreference;

    

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField codeField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //User Data variables
    [Header("UserData")]
    public TMP_InputField usernameField;
    public TMP_InputField roomSelect;
    public TMP_InputField profileName;
    
    //public Transform scoreboardContent;

    [Header("Profiles Button")]
    public GameObject Profile1;
    public GameObject Profile2;
    public GameObject Profile3;
    public GameObject Profile4;
    public GameObject Profile5;
    public GameObject Profile6;
    public GameObject AddProfileButton;

    public int numberOfAccount = 0;
    private List<GameObject> profileList = new List<GameObject>();

    /// <summary>
    /// enum created to keep track of profiles
    /// </summary>
    enum profile
    {
        PROFILE1, PROFILE2, PROFILE3, PROFILE4, PROFILE5, PROFILE6
    }

    private profile accountId = profile.PROFILE1;



    void Awake()
    {

        profileList.Add(Profile1);
        profileList.Add(Profile2);
        profileList.Add(Profile3);
        profileList.Add(Profile4);
        profileList.Add(Profile5);
        profileList.Add(Profile6);
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    //Function to Initialiaze Firebase
    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        DBreference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    //Function to clear Login fields
    public void ClearLoginFeilds()
    {
        emailLoginField.text = "";
        passwordLoginField.text = "";
    }


    //Function to clear Registration fields
    public void ClearRegisterFeilds()
    {
        usernameRegisterField.text = "";
        emailRegisterField.text = "";
        passwordRegisterField.text = "";
        passwordRegisterVerifyField.text = "";
    }

    //Function to quit Application
    public void QuitApplication()
    {
        Firebase.Auth.FirebaseUser user = auth.CurrentUser;
        if (user != null)
        {
            auth.SignOut();
        }
        Application.Quit();
    }

    //Function to add profiles
    public void addProfile()
    {
        profileName.text = "";

        UIManager.instance.CreateProfile();
    }


    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));

    }
    //Function for the sign out button
    public void SignOutButton()
    {
        auth.SignOut();
        UIManager.instance.LoginScreen();
        ClearRegisterFeilds();
        ClearLoginFeilds();
    }

    //Function for the save button
    public void SaveDataButton()
    {
        
        StartCoroutine(updateRoomSelection(int.Parse(roomSelect.text)));
        codeField.text = roomSelect.text;
        Debug.Log("Setting saved");
    }
    //Fucntion to save profiles
    public void saveProfileButton()
    {
        numberOfAccount++;
        if (numberOfAccount == 1)
        {
            accountId = profile.PROFILE1;
        }
        else if (numberOfAccount == 2)
        {
            accountId = profile.PROFILE2;
        }
        else if (numberOfAccount == 3)
        {
            accountId = profile.PROFILE3;
        }
        else if (numberOfAccount == 4)
        {
            accountId = profile.PROFILE4;
        }
        else if (numberOfAccount == 5)
        {
            accountId = profile.PROFILE5;
        }
        else
        {
            accountId = profile.PROFILE6;
        }
        //updates the number of profiles and profile name of new profile
        StartCoroutine(updateNumberOfProfiles(numberOfAccount));
        StartCoroutine(UpdateProfileName(profileName.text));
        profileList[numberOfAccount - 1].SetActive(true);
    
        StartCoroutine(accounts());
        Debug.Log(accountId);
        UIManager.instance.Profiles();
        startProfiles();

    }
    //displays the number of profiles created under the emailId 
    public void startProfiles()
    {


        if (numberOfAccount == 6)
        {
            AddProfileButton.SetActive(false);
        }
        else
        {
            AddProfileButton.SetActive(true);
        }

        for (int i = 0; i < numberOfAccount; i++)
        {
            profileList[i].SetActive(true);
        }
    }


    /// <summary>
    /// This fucntion takes in the email and password to log in.
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <returns></returns>
    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //handles errors if any
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            StartCoroutine(LoadUserData());
            StartCoroutine(accounts());
            yield return new WaitForSeconds(2);
            StartCoroutine(UpdateUsernameAuth(_email));
            StartCoroutine(UpdateUsernameDatabase(_email));

            usernameField.text = User.DisplayName;
            startProfiles();
            UIManager.instance.Profiles();

            confirmLoginText.text = "";
            ClearLoginFeilds();
            ClearRegisterFeilds();
        }
    }

    /// <summary>
    /// This function registers the user under the emailId, password and username provided.
    /// </summary>
    /// <param name="_email"></param>
    /// <param name="_password"></param>
    /// <param name="_username"></param>
    /// <returns></returns>
    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //handles errors if any
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //return to login screen
                        UIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        ClearRegisterFeilds();
                        ClearLoginFeilds();
                    }
                }
            }
        }
    }

    /// <summary>
    /// updates the username after the user logIn
    /// </summary>
    /// <param name="_username"></param>
    /// <returns></returns>
    private IEnumerator UpdateUsernameAuth(string _username)
    {
        //Create a user profile and set the username
        UserProfile profile = new UserProfile { DisplayName = _username };

        //Call the Firebase auth update user profile function passing the profile with the username
        var ProfileTask = User.UpdateUserProfileAsync(profile);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

        if (ProfileTask.Exception != null)
        {
            //Logs warning if any
            Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
        }
        else
        {
            //Auth username is now updated
        }
    }
    /// <summary>
    /// updates the username of the person in the database
    /// </summary>
    /// <param name="_username"></param>
    /// <returns></returns>
    private IEnumerator UpdateUsernameDatabase(string _username)
    {
        //Set the currently logged in user username in the database
        var DBTask = DBreference.Child("users").Child(User.UserId).Child("UserName").SetValueAsync(_username);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            //Logs warning if any
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database username is now updated
        }
    }
    /// <summary>
    /// This function updates up the name of person in profile.
    /// </summary>
    /// <param name="_profileName"></param>
    /// <returns></returns>
    private IEnumerator UpdateProfileName(string _profileName)
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).Child(accountId.ToString()).Child("profile_Name").SetValueAsync(_profileName);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            //Logs warning if any
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //Database profile name is now updated
        }

    }
    /// <summary>
    /// This function updates the numbers of profiles under the emailId
    /// </summary>
    /// <param name="_profiles"></param>
    /// <returns></returns>
    private IEnumerator updateNumberOfProfiles(int _profiles)
    {

        var DBTask = DBreference.Child("users").Child(User.UserId).Child("Number_of_profiles").SetValueAsync(_profiles);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            //Logs warning if any
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //number of profiles are now updated
        }
    }


    /// <summary>
    /// This function updates the room selected by the person
    /// </summary>
    /// <param name="_roomSelected"></param>
    /// <returns></returns>
    private IEnumerator updateRoomSelection(int _roomSelected)
    {
        //Set the currently logged in user room
        var DBTask = DBreference.Child("users").Child(User.UserId).Child(accountId.ToString()).Child("room_selected").SetValueAsync(_roomSelected);

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            //Logs warning if any
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else
        {
            //radio are now updated
        }
    }
    /// <summary>
    /// this account sets up the numberofAccount variable to the number of profiles in the emailId
    /// </summary>
    /// <returns></returns>
    private IEnumerator accounts()
    {
        var DBTask = DBreference.Child("users").Child(User.UserId).GetValueAsync();
        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            numberOfAccount = 0;
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;
            this.numberOfAccount = int.Parse(snapshot.Child("Number_of_profiles").Value.ToString());

        }

    }

    /// <summary>
    /// This function loads the user data from the firebase
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadUserData()
    {
        //Get the currently logged in user data
        var DBTask = DBreference.Child("users").Child(User.UserId).Child(accountId.ToString()).GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            //Logs warning if any
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
        }
        else if (DBTask.Result.Value == null)
        {
            //if result if null it sets roomSelected and username fields to 0 and NA respectively
            roomSelect.text = "0";
            usernameField.text = " NA ";

        }
        else
        {
            //otherwise shows the data retrieved
            DataSnapshot snapshot = DBTask.Result;
            roomSelect.text = snapshot.Child("room_selected").Value.ToString();
            usernameField.text = snapshot.Child("profile_Name").Value.ToString();
            

        }
    }
    
    
    //functions for different profile button
    //each function sets up the accountId to the id of the person selected and then loads it selections
    public void profileButton1()
    {
        this.accountId = profile.PROFILE1;
        StartCoroutine(LoadUserData());
  
        UIManager.instance.UserDataScreen();


    }
    public void profileButton2()
    {
        this.accountId = profile.PROFILE2;
        StartCoroutine(LoadUserData());
       
        UIManager.instance.UserDataScreen();

    }
    public void profileButton3()
    {
        this.accountId = profile.PROFILE3;
        StartCoroutine(LoadUserData());
       
        UIManager.instance.UserDataScreen();
    }
    public void profileButton4()
    {
        this.accountId = profile.PROFILE4;
        StartCoroutine(LoadUserData());
      
        UIManager.instance.UserDataScreen();
    }
    public void profileButton5()
    {
        this.accountId = profile.PROFILE5;
        StartCoroutine(LoadUserData());
        
        UIManager.instance.UserDataScreen();
    }
    public void profileButton6()
    {
        this.accountId = profile.PROFILE6;
        StartCoroutine(LoadUserData());
        UIManager.instance.UserDataScreen();
    }
    //end of profile buttons





}
