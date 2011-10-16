using UnityEngine;
using System.Collections;

public class ScrollingTextArea : MonoBehaviour {
	
	// Number of lines that will fit in a text area.
	public int maxLines = 3;

	// Should we wait for player input before moving to the next screen.
	public bool autoScrolling = true;
		
	// seconds between words being added
	public float scrollSpeed = 1.0f;
	
	
	// All the text that will be displayed in this text area.
	// Set through the Initialize function.
	private string text = "SHOULD NEVER BE SEEN!!!";
	
	// Track when new words need to be added.
	private float scrollTimer = 0.0f;
	
	// Keep ref to SpriteText component.
	private SpriteText spriteText;
	
	// Word Buffer
	private Queue wordQueue;
	
	// Is all the text done scrolling.
	private bool done = false;
	
	// Waiting for a button press to advance to next screen.
	private bool waitingForInput = false;
	
	// Waiting for final button press before firing the callbacks and destroying the text area
	private bool waitingForFinalInput = false;
	
	// GameObject to fire callback on
	private GameObject callbackObject = null;
	
	// Callback message
	private string callbackMessage = "";
	
	// Use this for initialization
	void Start () {
		
		// Set this to true because we don't want to start any "work" until
		// the Initialize function is called.
		done = true;
		
		wordQueue = new Queue();
		
		spriteText = this.gameObject.GetComponent<SpriteText>();
		if( spriteText != null )
		{						
			// Empty string to start.
			spriteText.Text = "";
		}		
		
		// TODO DEBUG
		Initialize( "Here's some test text that will be so long that it will have to roll over into multiple lines and give us a chance to test the autoscrolling functionality",
		           this.gameObject, "AllDone" );
	}
	
	void OnDestroy()
	{
		print("Destroying text area");
		wordQueue.Clear();
		spriteText.Delete();
	}
	
	
	// All the actual work starts in the Init function since we need to pass in arguments
	// before doing anything.
	void Initialize( string startingText, GameObject startingCallbackObject, string startingCallbackMessage )
	{
		text = startingText;
		callbackObject = startingCallbackObject;
		callbackMessage = startingCallbackMessage;
		
		string[] words = text.Split(' ');
		for( int i = 0; i < words.Length; i++ )
		{
			wordQueue.Enqueue( words[i] );
		}	
		
		done = false;
	}
		
	
	// Update is called once per frame
	void Update () {
				
		if( done )
			return;
		
		// Waiting for button press to fire callbacks and destroy this object.
		if( waitingForFinalInput )
		{
			bool inputReceived = false;
#if UNITY_EDITOR
			if( Input.GetButtonUp("Fire1") )
			{
				inputReceived = true;	
			}
#elif UNITY_ANDROID || UNITY_IPHONE		
			if( Input.touches.Length > 0 )
			{
				inputReceived = true;	
			}
#endif
			
			if( inputReceived )
			{
				FinishProcessing();
			}			
			return;
		}
		
		
		// Waiting for button press to advance to next screen.
		else if( waitingForInput )
		{
			bool inputReceived = false;
#if UNITY_EDITOR
			if( Input.GetButtonUp("Fire1") )
			{
				inputReceived = true;	
			}
#elif UNITY_ANDROID || UNITY_IPHONE		
			if( Input.touches.Length > 0 )
			{
				inputReceived = true;	
			}
#endif
			
			if( inputReceived )
			{
				waitingForInput = false;
				spriteText.Text = "";
			}			
			return;
		}
		
		scrollTimer = scrollTimer + Time.deltaTime;
		
		if( scrollTimer >= scrollSpeed )
		{			
			scrollTimer = 0;
			
			if( wordQueue.Count > 0 )
			{
				// See if the next word will push the text to the next
				// line and if so check if it's past the maximum.
				string nextWord = wordQueue.Peek() as string;
				int originalLength = spriteText.Text.Length;
				spriteText.Text += (nextWord + " ");
				
				// If we're not over the max, then dequeue the word
				// and continue on.
				if( spriteText.GetDisplayLineCount() <= maxLines )
				{
					wordQueue.Dequeue();	
				}
				
				// If we past the maximum lines, clear the text area and continue.
				else
				{		
					// If we're automatically moving onto the next screen,
					// then clear the text and continue going.
					if( autoScrolling )
					{
						spriteText.Text = "";
					}
					
					// Wait for player input before moving on to the next screen.
					else
					{
						// Remove the last word added
						spriteText.Text = spriteText.Text.Substring(0, originalLength);
						
						// Wait for a button press
						waitingForInput = true;
						
					}					
				}
			}
			
			// We just reached the end of the text.
			else
			{			
				// Don't wait to finish up the text area.
				if( autoScrolling )
				{
					FinishProcessing();
				}
				
				// If autoscroll is turned off, then wait for input before ending
				// the screen.
				else
				{
					if( !waitingForFinalInput )
					{
						// Wait for a button press
						waitingForFinalInput = true;	
					}
				}
			}
		}
	}
	
	
	// Call when the TextArea is done processing and ready to fire off the callbacks
	// and destroy itslef.
	void FinishProcessing()
	{
		waitingForFinalInput = false;
		// We're done here.
		done = true;
		string DUMMY_DATA = "";
		callbackObject.SendMessage( callbackMessage, DUMMY_DATA, SendMessageOptions.RequireReceiver);
		
		GameObject.Destroy( this.gameObject );	
	}
	
	void AllDone( string data )
	{
		print("All done");
	}
}
