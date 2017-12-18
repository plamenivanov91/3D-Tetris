using UnityEngine;

public class Tetramino {
	public Color color = new Color(Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f), Random.Range(0.0f,1.0f));
	private int currState = 0;
	private int possibleStates;
	private int[][] states;

	public int[][] States
	{
		get { return states; }
		set 
		{ 
			possibleStates = value.Length;
			states = value;
		}
	}

	public int getRandomState() {
		currState = Random.Range (0, possibleStates);
		return currState;
	}

	public int CurrentState{
		get { return currState; }
	}

	public void incrementState() {
		currState++;
		if (currState == possibleStates) {
			currState = 0;
		}
	}

	public void decrementState() {
		currState--;
		if (currState < 0) {
			currState = possibleStates - 1;
		}
	}
}

