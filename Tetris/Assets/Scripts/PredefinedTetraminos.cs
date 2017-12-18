public class PredefinedTetraminos {
	public static int[][][] tetraminosStates = new int[7][][];

	static void setStates(){
		// Predifed States of the Tetramino "T"
		tetraminosStates [0] = new int [4][];
		tetraminosStates [0][0] = new int[]{0,1,1,1,2,1,1,2};
		tetraminosStates [0][1] = new int[]{1,0,0,1,1,1,1,2};
		tetraminosStates [0][2] = new int[]{1,0,1,1,0,1,2,1};
		tetraminosStates [0][3] = new int[]{1,0,1,1,1,2,2,1};
		// Predifed States of the Tetramino "J"
		tetraminosStates [1] = new int [4][];
		tetraminosStates [1][0] = new int[]{1,0,1,1,0,2,1,2};
		tetraminosStates [1][1] = new int[]{0,0,0,1,1,1,2,1};
		tetraminosStates [1][2] = new int[]{1,0,1,1,1,2,2,0};
		tetraminosStates [1][3] = new int[]{0,1,1,1,2,1,2,2};
		// Predifed States of the Tetramino "L"
		tetraminosStates [2] = new int [4][];
		tetraminosStates [2][0] = new int[]{1,0,1,1,2,2,1,2};
		tetraminosStates [2][1] = new int[]{0,2,0,1,1,1,2,1};
		tetraminosStates [2][2] = new int[]{1,0,1,1,1,2,0,0};
		tetraminosStates [2][3] = new int[]{0,1,1,1,2,1,2,0};
		// Predifed States of the Tetramino "Z"
		tetraminosStates [3] = new int [2][];
		tetraminosStates [3][0] = new int[]{0,1,1,1,1,2,2,2};
		tetraminosStates [3][1] = new int[]{1,0,1,1,0,1,0,2};
		// Predifed States of the Tetramino "S"
		tetraminosStates [4] = new int [2][];
		tetraminosStates [4][0] = new int[]{1,0,2,0,0,1,1,1};
		tetraminosStates [4][1] = new int[]{1,0,1,1,2,1,2,2};
		// Predifed States of the Tetramino "I"
		tetraminosStates [5] = new int [2][];
		tetraminosStates [5][0] = new int[]{0,0,1,0,2,0,3,0};
		tetraminosStates [5][1] = new int[]{0,0,0,1,0,2,0,3};
		// Predifed State of the Tetramino "Square/Cube"
		tetraminosStates [6] = new int [1][];
		tetraminosStates [6][0] = new int[]{0,0,1,0,0,1,1,1};
	}

	public static int[][][] getStates(){
		setStates();
		return tetraminosStates;
	}
}