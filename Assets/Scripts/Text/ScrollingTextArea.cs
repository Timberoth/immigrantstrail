using UnityEngine;
using System.Collections;

public class ScrollingTextArea : MonoBehaviour {
	
	public int maxLines = 3;
	
	public float scrollSpeed = 1.0f;	
	private float scrollTimer = 0.0f;
	
	// Keep ref to SpriteText component.
	private SpriteText spriteText;
	
	// Word Buffer
	private Queue wordQueue;
	
	// Original text.
	private string originalText = null;
	
	
	// Should we wait for player input before moving to the next screen.
	public bool autoScrolling = true;
	
	private bool done = false;
	
	// Use this for initialization
	void Start () {
		
		wordQueue = new Queue();
		
		spriteText = this.gameObject.GetComponent<SpriteText>();
		if( spriteText != null )
		{
			print(spriteText.Text);
			originalText = spriteText.Text;
			
			// Null text
			spriteText.Text = "";			
		}
		
		string[] words = originalText.Split(' ');
		for( int i = 0; i < words.Length; i++ )
		{
			wordQueue.Enqueue( words[i] );
		}		
	}
	
	// Update is called once per frame
	void Update () {
				
		if( done )
			return;
		
		scrollTimer = scrollTimer + Time.deltaTime;
		
		if( scrollTimer >= scrollSpeed )
		{			
			scrollTimer = 0;
			
			if( wordQueue.Count > 0 )
			{
				// See if the next word will push the text to the next
				// line and if so check if it's past the maximum.
				string nextWord = wordQueue.Peek() as string;
				spriteText.Text += (nextWord + " ");
				
				// If we're not over the max, then dequeue the word
				// and continue on.
				if( spriteText.GetDisplayLineCount() < maxLines )
				{
					wordQueue.Dequeue();	
				}
				
				// If we past the maximum lines, clear the text area and continue.
				else
				{		
					if( autoScrolling )
					{
						spriteText.Text = "";
					}
					
					// Wait for player input before moving on to the next screen.
					else
					{
						
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
			}
		}
	}
}
