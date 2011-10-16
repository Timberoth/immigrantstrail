using UnityEngine;
using System.Collections;

public class ScrollingTextArea : MonoBehaviour {
	
	// Number of lines that will fit in a text area.
	public int maxLines = 3;

	// All the text that will be displayed in this text area.
	public string text = "This is just some test text that will be replaced later when strings are passed in through a function";

	// Should we wait for player input before moving to the next screen.
	public bool autoScrolling = true;
		
	// seconds between words being added
	public float scrollSpeed = 1.0f;
	
	
	
	// Track when new words need to be added.
	private float scrollTimer = 0.0f;
	
	// Keep ref to SpriteText component.
	private SpriteText spriteText;
	
	// Word Buffer
	private Queue wordQueue;
	
	// Is all the text done scrolling.
	private bool done = false;
	
	// Waiting for a button press
	private bool waitingForInput = false;
	
	
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
		Initialize( "Here's some test text", this.gameObject, "AllDone" );
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
		
		if( waitingForInput )
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
				// NEED TO FIGURE OUT WHAT TO DO HERE.
				done = true;
				
				// Fire Callback
				print("Fire Callback");
				
				//GameObject.Destroy( this.gameObject );
				callbackObject.SendMessage( callbackMessage, "callback test", SendMessageOptions.RequireReceiver);
			}
		}
	}
	
	
	void AllDone( string data )
	{
		print( data );
		print("All done");
		
		GameObject.Destroy( this.gameObject );
	}
}
