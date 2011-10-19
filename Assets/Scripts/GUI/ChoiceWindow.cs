using UnityEngine;
using System.Collections;

public class ChoiceWindow : MonoBehaviour {
	
	// Choice Text refs
	private SpriteText[] choices;
	
	// Question Text ref 
	private SpriteText question;
	
	// Index of the current selected choice
	private int selection = 0;
	
	// GameObjects that may be message senders if the choice is selected.
	// Must be filled in by Initalize function.
	private GameObject[] messageSenders;
	private string[] messages;
	private string[] messageData;
	
	
	// Audio Ref
	private AudioSource highlightSound = null;
	private AudioSource selectionSound = null;
	
	
	// Use this for initialization
	void Start () {
		// Create ChoiceObjects array
		choices = new SpriteText[4];
		
		// Grab references to the child objects within the ChoiceWindow
		Transform tempTransform;
		
		// Question object
		tempTransform = transform.FindChild( "QuestionText" );
		question = tempTransform.GetComponent<SpriteText>();
		question.Text = "";
		
		// Choice objects
		string objectName;
		for( int i = 0; i < choices.Length; i++ )
		{
			objectName = "ChoiceButton"+(i+1);
			tempTransform = transform.FindChild( objectName );
			choices[i] = tempTransform.GetComponentInChildren<SpriteText>();			
			
			// Start with no text
			choices[i].Text = "";			
		}
		
		
		// Initalize audio
		InitializeAudio();
		
		// THIS WINDOW IS USELESS UNTIL Initialize() is called.
		
		// TODO TESTING ONLY
		string testQuestion = "How do you want to travel?";
		string[] testChoices = {"Airplane","Boat","On Foot","Truck"};
		GameObject[] testSenders = { gameObject, gameObject, gameObject, gameObject };
		string[] testMessages = {"AllDone","AllDone","AllDone","AllDone"};
		string[] testData = {"Your plane crashed","Your boat sank","You fell into a pit","Your truck exploded"};
		Initialize( testQuestion, testChoices, testSenders, testMessages, testData );
	}
	

	// Initialize this window with the real content.
	void Initialize( string question, string[] choices, GameObject[] senders, string[] messages, string[] datas )
	{
		this.question.Text = question;
		
		// Assuming startingChoices length is 4
		for( int i = 0; i < choices.Length; i++ )
		{
			this.choices[i].Text = choices[i];
		}
		
		messageSenders = senders;
		this.messages = messages;
		messageData = datas;
		
		// First choice is selected by default
		selection = 0;
		this.choices[selection].SetColor( Color.yellow );
	}
	
	
	void InitializeAudio()
	{
		// Create references to the attached audio sources.
		foreach( AudioSource source in this.GetComponents<AudioSource>() )
		{			
			if( source.clip.name == "ChoiceHighlight" )
			{
				highlightSound = source;				
			}			
			else if( source.clip.name == "ChoiceSelection" )
			{
				selectionSound = source;				
			}			
		}		
	}
	
	
	// Deconstructor
	void OnDestroy()
	{
		// Destory all SpriteText to avoid leaking memory
		question.Delete();
		for( int i = 0; i < choices.Length; i++ )
		{
			choices[i].Delete();
			choices[i] = null;
		}
	}
	
	
	// Update is called once per frame
	void Update () {		
	}
	
	
	void ChangeChoice( int newSelection )
	{
		// Play sound
		if( highlightSound != null )
		{
			AudioSource.PlayClipAtPoint(highlightSound.clip, Vector3.zero);			
		}
		
		// Play particle effect
		
		// Deselect the last choice
		choices[selection].SetColor( Color.white );
		
		// Update the selection
		selection = newSelection;
		
		// Select the new choice
		choices[selection].SetColor( Color.yellow );
	}
	
	
	// Choose the currently selected choice.
	void ChooseAnswer()
	{
		// Play sound
		if( selectionSound != null )
		{
			AudioSource.PlayClipAtPoint(selectionSound.clip, Vector3.zero);			
		}
		
		// Play particle effect
		
		// Fire the associate callback/s.
		messageSenders[selection].SendMessage( messages[selection], messageData[selection], SendMessageOptions.DontRequireReceiver);
		
		
		// Close this choice window.
		GameObject.Destroy( this.gameObject );
	}
	
	
	// TODO Debug function to test the callbacks are being called.
	void AllDone( string data )
	{
		print( data );
	}
		
	
	void Choice1Tap()
	{
		// Double tap means this choice is the answer
		if( selection == 0 )
			ChooseAnswer();
		else
			ChangeChoice( 0 );
	}
	
	void Choice2Tap()
	{
		if( selection == 1 )
			ChooseAnswer();
		else
			ChangeChoice( 1 );
	}
	
	void Choice3Tap()
	{
		if( selection == 2 )
			ChooseAnswer();
		else
			ChangeChoice( 2 );
	}
	
	void Choice4Tap()
	{
		if( selection == 3 )
			ChooseAnswer();
		else
			ChangeChoice( 3 );
	}
}
