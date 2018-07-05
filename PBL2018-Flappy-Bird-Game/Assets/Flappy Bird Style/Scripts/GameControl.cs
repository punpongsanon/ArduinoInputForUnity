using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour 
{
	public static GameControl instance;			//A reference to our game control script so we can access it statically.
	public Text scoreText;						//A reference to the UI text component that displays the player's score.
	public GameObject gameOvertext;				//A reference to the object that displays the text which appears when the player dies.

	private int score = 0;						//The player's score.
	public bool gameOver = false;				//Is the game over?
	public float scrollSpeed = -1.5f;
    public ArduinoConnector connect;
    private int dataX = 0;
    private int dataY = 0;
    private int dataW = 0;
    private int dataH = 0;
    public bool pushing = false;                //This is handle for pushing received by arduino > send to 'Bird.cs'

    void Awake()
	{
        //If we don't currently have a game control...
        if (instance == null)
        {
            //...set this one to be it...
            instance = this;
            connect.Open();
        }
        //...otherwise...
        else if (instance != this)
        {
            //...destroy this one because it is a duplicate.
            Destroy(gameObject);
            connect.Close();
        }
	}

	void Update()
	{
		//If the game is over and the player has pressed some input...
		if (gameOver && Input.GetMouseButtonDown(0)) 
		{
			//...reload the current scene.
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

        // Add Arduino
        connect.WriteToArduino("PING");
        string s = connect.ReadFromArduino(10000);
        //Debug.Log(s);
        string[] block = s.Split(null);
        dataX = int.Parse(block[0]);
        dataY = int.Parse(block[1]);
        dataW = int.Parse(block[2]);
        dataH = int.Parse(block[3]);

        // Log file for data received from Arduino > output in the console of Unity3D
        //Debug.Log("x: " + dataX + " y: " + dataY + " w: " + dataW + " h: " + dataH);
        //Debug.Log("value1: " + (dataX - dataW) + " value2: " + (dataY - dataH));

        // If size of block bigger than 15, then it is recognized as a pushing interaction
        // This information read by 'Bird.cs'
        if (dataX - dataW > 15 && dataY - dataH > 15)
        {
            pushing = true;
        }
        else
            pushing = false;

        //Debug.Log(pushing);
    }

	public void BirdScored()
	{
		//The bird can't score if the game is over.
		if (gameOver)	
			return;
		//If the game is not over, increase the score...
		score++;
		//...and adjust the score text.
		scoreText.text = "Score: " + score.ToString();
	}

	public void BirdDied()
	{
		//Activate the game over text.
		gameOvertext.SetActive(true);
		//Set the game to be over.
		gameOver = true;
    }

    void OnApplicationQuit()
    {
        connect.Close();
    }
}
