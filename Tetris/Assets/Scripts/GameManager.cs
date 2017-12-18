using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public int rows = 20;
	public int cols = 10;
	public Transform squarePrefab;
	public bool isWithStupidAI = false;
	private bool[,] takenMatrixPositions;
	private Tetramino currTetramino;
	private int currTetraminoState;
	private int[][][] predefinedStates = PredefinedTetraminos.getStates ();
	private Tetramino[] tetraminos;
	private int currCoordIndex = 0;
	private List<GameObject> currentCubesArr = new List<GameObject> ();
	private List<GameObject> allCubesArr = new List<GameObject> ();
	private float tickInterval = 1f;
	private float tickIncreaseSlowStep = 0.2f;
	private int initialTetraminoYAddition;
	private int[] currTetraminoCoords = new int[8];
	private bool controlsEnabled = true;
	private int horizontalAddition = 0;
	private int verticalAddition = 0;
	private string[] possibleDirections = { "down", "left", "right" };
	private bool isFirstRotation = true;
	private bool isPaused = false;
	private bool isGameOver = false;
	Coroutine lastRoutine = null;

	void Start ()
	{ 
		print ("Started");
		takenMatrixPositions = new bool[rows, cols];
		initialTetraminoYAddition = (cols / 2) - 3;
		tetraminos = new Tetramino[predefinedStates.Length];

		for (int r = 0; r < rows; r++) {
			for (int c = 0; c < cols; c++) {
				Transform instance = Instantiate (squarePrefab, new Vector3 (r, c, 0), Quaternion.identity) as Transform;
				instance.transform.SetParent (transform.Find ("Manager"));
			}
		}

		for (int i = 0; i < predefinedStates.Length; i++) {
			Tetramino currTetramino = new Tetramino ();
			currTetramino.States = predefinedStates [i];
			tetraminos [i] = currTetramino;
		}


		GenerateNewTetramino ();
		if (isWithStupidAI) {
			controlsEnabled = false;
		}
		lastRoutine = StartCoroutine (MoveTetraminos ());
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.P) && !isGameOver) { //Pause game
			print (isPaused ? "GAME UNPAUSED" : "GAME PAUSED");
			PauseGame ();
		}
		else if (Input.GetKeyDown (KeyCode.R)) //Restart game
			RestartGame();
		if (controlsEnabled) {
			if (Input.GetKeyDown (KeyCode.Q)) { //Slows drop/tick speed
				tickInterval += tickIncreaseSlowStep;
			} else if (Input.GetKeyDown (KeyCode.E)) { //Increases drop/tick speed
				if (tickInterval > tickIncreaseSlowStep) {
					tickInterval -= tickIncreaseSlowStep;
				}
			} else if (Input.GetKeyDown (KeyCode.Z)) { //Counterclockwise rotation
				RotateTetramino (true);
			} else if (Input.GetKeyDown (KeyCode.C)) { //Clockwise rotation
				RotateTetramino (false);
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
				if (UpdateCurrentNumeralPosition ("right"))
					UpateCurrentCubesPosition ();
			} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				if (UpdateCurrentNumeralPosition ("left"))
					UpateCurrentCubesPosition ();
			} else if (Input.GetKey (KeyCode.DownArrow)) { //Quicker drop/tick speed
				if (UpdateCurrentNumeralPosition ("down"))
					UpateCurrentCubesPosition ();
			}
		}
	}
		
	IEnumerator MoveTetraminos ()
	{
		while (true) {
			if (!isGameOver) {
				if (isWithStupidAI)
				if (UpdateCurrentNumeralPosition (possibleDirections [Random.Range (0, possibleDirections.Length)]))
					UpateCurrentCubesPosition ();
				
				if (UpdateCurrentNumeralPosition ("down"))
					UpateCurrentCubesPosition ();
				yield return new WaitForSeconds (tickInterval);
			} else {
				yield return new WaitForFixedUpdate();
			}
		}
	}

	void RotateTetramino (bool isClockwise)
	{
		if (isFirstRotation) {
			horizontalAddition += initialTetraminoYAddition;
			isFirstRotation = false;
		}

		if (isClockwise)
			currTetramino.incrementState ();
		else
			currTetramino.decrementState ();

		if (isPositionAvailable (currTetramino.States [currTetramino.CurrentState])) {
			System.Array.Copy (currTetramino.States [currTetramino.CurrentState], currTetraminoCoords, 8);
		}else{
			if(isClockwise)
				currTetramino.decrementState();
			else
				currTetramino.incrementState();
		}
		UpateCurrentCubesPosition ();
	}

	void GenerateNewTetramino ()
	{
		currentCubesArr = new List<GameObject> ();
		currTetramino = tetraminos [Random.Range (0, tetraminos.Length)];
		currTetraminoState = currTetramino.getRandomState ();

		currTetraminoCoords = new int[8];
		horizontalAddition = initialTetraminoYAddition;
		isFirstRotation = true;
		verticalAddition = 0;
		for (int i = 0; i < currTetramino.States [currTetraminoState].Length; i += 2) {
			currTetraminoCoords [i] = currTetramino.States [currTetraminoState] [i];
			currTetraminoCoords [i + 1] = currTetramino.States [currTetraminoState] [i + 1] + horizontalAddition;
			if (takenMatrixPositions [currTetraminoCoords [i], currTetraminoCoords [i + 1]]) {
				isGameOver = true;
				controlsEnabled = false;
				print ("GAME OVER");
				break;
			}
		}
		if (!isGameOver) {
			for (int i = 0; i < 4; i++) {
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				cube.GetComponent<Renderer> ().material.color = currTetramino.color;
				currentCubesArr.Add (cube);
			}
			UpateCurrentCubesPosition ();
		}
	}

	bool UpdateCurrentNumeralPosition (string directionOfMovement)
	{
		bool isPassing = true;

		switch (directionOfMovement) {
		case "down":
			verticalAddition++;
			break;
		case "left":
			horizontalAddition--;
			break;
		case "right":
			horizontalAddition++;
			break;
		}

		if (!isPositionAvailable (currTetraminoCoords)) {
			switch (directionOfMovement) {
			case "down":
				verticalAddition--;
				PreparationForNextTetramino ();
				break;
			case "left":
				horizontalAddition++;
				break;
			case "right":
				horizontalAddition--;
				break;
			}
			isPassing = false;
		}

		return isPassing;
	}

	void PreparationForNextTetramino ()
	{
		StopCoroutine(lastRoutine);
		EndLifeOfTetramino ();
		CheckForSuccessfulLanes ();
		GenerateNewTetramino ();
		if(!isGameOver)
			lastRoutine = StartCoroutine (MoveTetraminos ());
	}

	void CheckForSuccessfulLanes ()
	{
		bool isLane;
		List<int> lanesToDestroy = new List<int> ();

		for (int i = rows - 1; i >= 0; i--) {
			if (lanesToDestroy.Count > 0) {
				if (lanesToDestroy [lanesToDestroy.Count - 1] - lanesToDestroy [0] == 3) {
					break;
				}
			}
			isLane = true;
			for (int j = 0; j < cols; j++) {
				if (!takenMatrixPositions [i, j]) {
					isLane = false;
					break;
				}
			}
			if (isLane)
				lanesToDestroy.Add (i);
		}

		if (lanesToDestroy.Count > 0)
			DestroySuccessfulLanes (lanesToDestroy);
	}

	void DestroySuccessfulLanes (List<int> lanesToDestroy)
	{
		int lanesToDestroyLength = lanesToDestroy.Count;
		int currRow;
		for (int i = 0; i < lanesToDestroyLength; i++) {
			currRow = lanesToDestroy [i];
			for (int j = 0; j < cols; j++) {
				takenMatrixPositions [currRow, j] = false;
				for (int c = 0; c < allCubesArr.Count; c++) {
					if (allCubesArr [c].transform.position.x == currRow) {
						Destroy (allCubesArr [c]);
						allCubesArr.RemoveAt (c);
					}
				}
			}
		}

		RearrangeElements (lanesToDestroy);
	}
	 
	void RearrangeElements (List<int> lanesToFill)
	{
		int stepsNeeded = 1;
		int passedColapsedRows = 0;
		int lanesCount = lanesToFill.Count;
		for (int i = 0; i < lanesCount - 1; i++) {
			int currLane = lanesToFill [i];
			int nextLane = lanesToFill [i + 1];
			if (currLane - 1 != nextLane) {
				passedColapsedRows += stepsNeeded;
				for (int j = 0; j < currLane - nextLane - 1; j++)
					MoveDownCubes (currLane - 1 - j, currLane - 1 - j, stepsNeeded);
				stepsNeeded = 1;
			} else {
				stepsNeeded++;
			}
		}

		int topRow = FindTopRowOfTheStackedTetraminos ();// + passedColapsedRows;
		MoveDownCubes (lanesToFill [lanesToFill.Count - 1] - 1, topRow, stepsNeeded + passedColapsedRows);
	}

	void MoveDownCubes (int bottomRow, int topRow, int stepsNeeded)
	{
		int allCubesLength = allCubesArr.Count;
		for (int i = bottomRow; i >= topRow; i--) {
			for (int j = 0; j < cols; j++) {
				if (takenMatrixPositions [i, j]) {
					takenMatrixPositions [i + stepsNeeded, j] = true;
					takenMatrixPositions [i, j] = false;
				}
			}
			for (int c = 0; c < allCubesLength; c++) {
				if (allCubesArr [c].transform.position.x == i)
					allCubesArr [c].transform.position = new Vector3 (i + stepsNeeded, allCubesArr [c].transform.position.y, allCubesArr [c].transform.position.z);
			}
		}
	}

	int FindTopRowOfTheStackedTetraminos ()
	{
		for (int i = 0; i < rows; i++)
			for (int j = 0; j < cols; j++)
				if (takenMatrixPositions [i, j])
					return i;
		return 69;
	}

	void EndLifeOfTetramino ()
	{
		for (int i = 0; i < currTetraminoCoords.Length; i += 2)
			takenMatrixPositions [currTetraminoCoords [i] + verticalAddition, currTetraminoCoords [i + 1] + horizontalAddition] = true;

		for (int i = 0; i < currentCubesArr.Count; i++)
			allCubesArr.Add (currentCubesArr [i]);
	}

	void UpateCurrentCubesPosition ()
	{
		for (int i = 0; i < 4; i++) {
			currentCubesArr [i].transform.position = new Vector3 (
				currTetraminoCoords [currCoordIndex] + verticalAddition,
				currTetraminoCoords [currCoordIndex + 1] + horizontalAddition,
				-1.5f
			);
			currCoordIndex += 2;
		}
		currCoordIndex = 0;
	}

	bool isPositionAvailable(int[] newPositions){
		int newPosLength = newPositions.Length;
		for (int i = 0; i < newPosLength; i += 2) {
			try{
				if(takenMatrixPositions [newPositions [i] + verticalAddition, newPositions [i + 1] + horizontalAddition])
					return false;
			}catch{
				return false;
			}
		}
		return true;
	}

	void PauseGame(){
		
		if (isPaused)
			lastRoutine = StartCoroutine (MoveTetraminos());
		else
			StopCoroutine (lastRoutine);
		
		controlsEnabled = isPaused;
		isPaused = !isPaused;
	}

	void RestartGame(){
		
		if (isGameOver) {
			isGameOver = false;
			controlsEnabled = true;
		}
			
		int topRow = FindTopRowOfTheStackedTetraminos();
		for (int i = rows - 1; i >= topRow; i--) {
			for (int j = 0; j < cols; j++) {
				if (takenMatrixPositions [i, j])
					takenMatrixPositions [i, j] = false;
			}
		}

		foreach (GameObject cube in allCubesArr)
			Destroy (cube);

		foreach (GameObject cube in currentCubesArr)
			Destroy (cube);
		
		allCubesArr.Clear ();
		currentCubesArr.Clear ();
		GenerateNewTetramino ();

		print ("GAME RESTARTED!");
	}
}