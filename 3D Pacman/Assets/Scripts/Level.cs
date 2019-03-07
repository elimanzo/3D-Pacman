using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : MonoBehaviour {

	/* 
		Level is: 28 tiles wide (c/x/w) and 31 tiles high (r/z/d) - Original Game
	 	(screen is 36 tiles high) 	- 3 @ top for score/highscore 
		 							- 2 @ bottom for lives and fruit collected
									 
		a - upper right corner 					B - Blinky spawn 
		b - bottom right corner 				P - Pinky spawn 
		c - bottom left corner  				I - Inky spawn
		d - upper left corner 					C - Clyde spawn 
		w - warp (matches opposite w)           S - PacMan and Fruit spawn
		x - horozantal wall 
		y - vertical wall						. - dot 
		z - ghost house doors 					* - energizer
		
		d x x x x x x x x x x x x a d x x x x x x x x x x x x a
		y . . . . . . . . . . . . y y . . . . . . . . . . . . y
		y . d x x a . d x x x a . y y . d x x x a . d x x a . y 
		y * y     y . y       y . y y . y       y . y     y * y
		y . c x x b . c x x x b . c b . c x x x b . c x x b . y
        y . . . . . . . . . . . . . . . . . . . . . . . . . . y
		y . d x x a . d a . d x x x x x x a . d a . d x x a . y
		y . c y y b . y y . c x x a d x x b . y y . c x x b . y
		y . . . . . . y y . . . . y y . . . . y y . . . . . . y
		c x x x x a . y c x x a   y y   d x x b y . d x x x x b
                  y . y d x x b   c b   c x x a y . y
				  y . y y                     y y . y
				  y . y y   d x x z z x x a   y y . y
	    x x x x x b . c b   y             y   c b . c x x x x x 
        w           .       y   B P I C   y       .           w  
        x x x x x a . d a   y             y   d a . d x x x x x 
			      y . y y   c x x x x x x b   y y . y
				  y . y y                     y y . y
		          y . y y   d x x x x x x a   y y . y 
        d x x x x b . c b   c x x a d x x b   c b . c x x x x a
        y . . . . . . . . . . . . y y . . . . . . . . . . . . y
		y . d x x a . d x x x a . y y . d x x x a . d x x a . y
		y . c x a y . c x x x b . c b . c x x x b . y d x b . y
		y * . . y y . . . . . . . S   . . . . . . . y y . . * y
		c x a . y y . d a . d x x x x x x a . d a . y y . d x b
        d x b . c b . y y . c x x a d x x b . y y . c b . c x a
        y . . . . . . y y . . . . y y . . . . y y . . . . . . y
		y . d x x x x b c x x a . y y . d x x b c x x x x a . y
		y . c x x x x x x x x b . c b . c x x x x x x x x b . y
		y . . . . . . . . . . . . . . . . . . . . . . . . . . y
		c x x x x x x x x x x x x x x x x x x x x x x x x x x b
	*/ 

	
	public int MaxSize { get { return m_levelDepth * m_levelWidth; }} 
	public bool showPathfindingGrid = true; // pathfinding 
	public bool wireframe = false; // pathfinding
	private Node[,] m_grid; // pathfinding
	public float m_nodeDiameter = 1f; // pathfinding
	
	
	private const int m_levelDepth = 33; // rows 
	private const int m_levelWidth = 28; // columns 
	private char[,] m_levelMap = new char[m_levelDepth, m_levelWidth] { 
			{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
			{ 'd','x','x','x','x','x','x','x','x','x','x','x','x','a','d','x','x','x','x','x','x','x','x','x','x','x','x','a'}, 
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','a','.','d','x','x','x','a','.','y','y','.','d','x','x','x','a','.','d','x','x','a','.','y'}, 
			{ 'y','*','y',' ',' ','y','.','y',' ',' ',' ','y','.','y','y','.','y',' ',' ',' ','y','.','y',' ',' ','y','*','y'}, 
			{ 'y','.','c','x','x','b','.','c','x','x','x','b','.','c','b','.','c','x','x','x','b','.','c','x','x','b','.','y'}, 
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','a','.','d','a','.','d','x','x','x','x','x','x','a','.','d','a','.','d','x','x','a','.','y'}, 
			{ 'y','.','c','x','x','b','.','y','y','.','c','x','x','a','d','x','x','b','.','y','y','.','c','x','x','b','.','y'}, 
			{ 'y','.','.','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','.','.','y'}, 
			{ 'c','x','x','x','x','a','.','y','c','x','x','a',' ','y','y',' ','d','x','x','b','y','.','d','x','x','x','x','b'}, 
			{ ' ',' ',' ',' ',' ','y','.','y','d','x','x','b',' ','c','b',' ','c','x','x','a','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ','d','x','x','z','z','x','x','a',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ 'x','x','x','x','x','b','.','c','b',' ','y',' ',' ',' ',' ',' ',' ','y',' ','c','b','.','c','x','x','x','x','x'}, 
			{ 'w',' ',' ',' ',' ',' ','.',' ',' ',' ','y',' ','B','P','I','C',' ','y',' ',' ',' ','.',' ',' ',' ',' ',' ','w'}, 
			{ 'x','x','x','x','x','a','.','d','a',' ','y',' ',' ',' ',' ',' ',' ','y',' ','d','a','.','d','x','x','x','x','x'}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ','c','x','x','x','x','x','x','b',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ ' ',' ',' ',' ',' ','y','.','y','y',' ','d','x','x','x','x','x','x','a',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
			{ 'd','x','x','x','x','b','.','c','b',' ','c','x','x','a','d','x','x','b',' ','c','b','.','c','x','x','x','x','a'}, 
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','a','.','d','x','x','x','a','.','y','y','.','d','x','x','x','a','.','d','x','x','a','.','y'}, 
			{ 'y','.','c','x','a','y','.','c','x','x','x','b','.','c','b','.','c','x','x','x','b','.','y','d','x','b','.','y'}, 
			{ 'y','*','.','.','y','y','.','.','.','.','.','.','.','s',' ','.','.','.','.','.','.','.','y','y','.','.','*','y'}, 
			{ 'c','x','a','.','y','y','.','d','a','.','d','x','x','x','x','x','x','a','.','d','a','.','y','y','.','d','x','b'}, 
			{ 'd','x','b','.','c','b','.','y','y','.','c','x','x','a','d','x','x','b','.','y','y','.','c','b','.','c','x','a'}, 
			{ 'y','.','.','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','.','.','y'}, 
			{ 'y','.','d','x','x','x','x','b','c','x','x','a','.','y','y','.','d','x','x','b','c','x','x','x','x','a','.','y'},  
			{ 'y','.','c','x','x','x','x','x','x','x','x','b','.','c','b','.','c','x','x','x','x','x','x','x','x','b','.','y'},  
			{ 'y','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
			{ 'c','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','b'},
			{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}
	}; 
	
	// private const int m_levelDepth = 42; // rows 
	// private const int m_levelWidth = 64; // columns 
	
	// private char[,] m_levelMap = new char[m_levelDepth, m_levelWidth] { 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','d','x','x','x','x','x','x','x','x','a',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','d','x','x','x','x','x','x','a','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','c','x','x','x','x','x','x','b','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','c','x','x','x','x','x','x','x','x','b',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','d','x','x','x','x','x','x','x','a','d','x','x','x','x','x','x','x','x','x','a','d','x','x','x','x','x','x','x','a','d','x','x','x','x','x','x','x','x','a',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','.','.','.','.','.','.',' ',' ','.','.','.','.','.','.','.','.','.',' ',' ','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','d','x','x','x','a','.','y','c','x','x','x','a','.','d','x','x','x','b','y','.','d','x','x','x','a','.','y','y','.','d','x','x','x','x','a','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','y',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ','y','.','y','y','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','c','x','x','x','b','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','c','x','x','x','b','.','b','y','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '},
	// 		{ ' ',' ','y','.','.','.','.','.','.','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','.','.','.','.','.','.',' ',' ','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','d','x','x','x','x','x','b',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','d','x','x','x','a','.','a','y','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ','y','.','y','y','.','y',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','c','x','x','x','b','.','y','y','.','c','x','x','x','x','b','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','y','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','.','y',' ',' ',' ',' ','y','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ','c',' ','b',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','c','x','b',' ',' ',' ',' ','c','x','x','x','x','x','x','x','b','c','x','x','x','x','x','x','x','x','b',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ','d','x','x','x','x','a','d','x','x','x','x','a','d','x','a','d','x','a','d','x','x','x','a',' ',' ',' ',' ',' ',' ',' ',' ',' ','d','x','x','x','x','x','x','x','x','a',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ','y','d','x','x','x','b','y','d','x','x','a','y','y','d','a','d','a','y','y','d','x','x','b',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','d','x','x','x','x','x','x','a','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ','s',' ',' ',' ',' ',' ','y','y','d','x','x','a','y','y',' ',' ','y','y','y','y','y','y','y','y','y','c','x','x','a',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','c','x','a','y','y','c','x','x','b','y','y','y','c','b','y','y','y','d','x','x','b',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ','y','c','x','x','b','y','y','d','x','x','a','y','y','y',' ',' ','y','y','y','c','x','x','a',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ','c','x','x','x','x','b','c','b',' ',' ','c','b','c','b',' ',' ','c','b','c','x','x','x','b',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','d','a','d','x','x','x','x','a','d','x','a','d','x','a',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','y','d','x','x','a','y','y','d','a','d','a','y',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','y','y',' ',' ','y','y','y','y','y','y','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','y','c','x','x','b','y','y','y','c','b','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','c','x','x','x','x','x','x','b','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '},  
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','d','x','b','y','y','d','x','x','a','y','y','y',' ',' ','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ','c','x','x','x','x','x','x','x','x','b',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '},  
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','c','x','x','b','c','b',' ',' ','c','b','c','b',' ',' ','c','b',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}, 
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '},
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '},
	// 		{ ' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '}
	// }; 

	
	// Prefabs //TODO: Compare this way performance to ResourceLoad performance 
	public GameObject corner;
	public GameObject wall;
	public GameObject warp;
	public GameObject houseDoor;
	public GameObject dot;
	public GameObject energizer; 
	public GameObject ghost; 
	public GameObject player;
	public LayerMask unwalkableMask; // pathfinding
	
	// keep it neat in the heirarchy
	public Transform levelContainer; 
	private Transform m_ghostContainer;
	private Transform m_wallContainer;	
	private Transform m_dotContainer;
	private Transform m_energizerContainer;
	private Transform m_warpContainer; 
	// dots and energizer counters 
	private int m_dCount = 0;
	private int m_eCount = 0;
	// pathfinding
	public Transform m_pacMan;
	public Transform m_blinky;
	public Transform m_pinky;
	public Transform m_inky;
	public Transform m_clyde;


	private List<GameObject> m_ghosts;
	private bool m_levelBuilt = false; 
	private AudioSource bgSound; 

	void Start () {
		m_ghosts = new List<GameObject>();
		if (!m_levelBuilt) {
			BuildContainers();
			BuildLevel();
		} 
		bgSound = GetComponent<AudioSource>();
		bgSound.Play();
	}

	void BuildContainers() {
		// setup the containers to keep things neat 
		GameObject tmpContainer = new GameObject();
		
		tmpContainer.name = "Ghosts";
		tmpContainer.transform.parent = levelContainer;
		m_ghostContainer = tmpContainer.transform;

		tmpContainer = new GameObject();
		tmpContainer.name = "Dots";
		tmpContainer.transform.parent = levelContainer;
		m_dotContainer = tmpContainer.transform;

		tmpContainer = new GameObject();
		tmpContainer.name = "Energizers";
		tmpContainer.transform.parent = levelContainer;
		m_energizerContainer = tmpContainer.transform;
		
		tmpContainer = new GameObject();
		tmpContainer.name = "Warps";
		tmpContainer.transform.parent = levelContainer;
		m_warpContainer = tmpContainer.transform;

		tmpContainer = new GameObject();
		tmpContainer.name = "Walls";
		tmpContainer.transform.parent = levelContainer;
		m_wallContainer = tmpContainer.transform;
	} 

	void BuildLevel() {
		
		Debug.Log("Building Level");
		
		GameObject tmp; 
		bool danglingWarp = false;
		GameObject unPairedWarp = null;
		m_ghosts = new List<GameObject>();
		
		// pathfinding grid
		m_grid = new Node[m_levelDepth, m_levelWidth];

		// level 
		for(int r = 0, z = m_levelDepth - 1; r < m_levelDepth; r++, z--) {
			for (int c = 0, x = 0; c < m_levelWidth; c++, x++) {
				switch(m_levelMap[r,c]) {
					case 'a': // corner 180 rotation (top right)
						tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 180, 0)));
						tmp.name = "corner";
						tmp.transform.parent = m_wallContainer;
						break;
					case 'b': // corner -90 rotation (bottom right)
						tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, -90, 0)));
						tmp.name = "corner";
						tmp.transform.parent = m_wallContainer;
						break;
					case 'c': // no rotation (bottom left) 
						tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 0, 0)));
						tmp.name = "corner";
						tmp.transform.parent = m_wallContainer;
						break;
					case 'd': // corner 90 rotation (top left) 
						tmp = Instantiate(corner, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));
						tmp.name = "corner";
						tmp.transform.parent = m_wallContainer;
						break;
					case 'w': // warp
						tmp = Instantiate(warp, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 0, 0)));
						tmp.name = "warp";
						
						if (!danglingWarp) {
							unPairedWarp = tmp; 
							danglingWarp = true;
						} else { 
							unPairedWarp.GetComponent<Warp>().setWarpMate(tmp);
							unPairedWarp.GetComponent<Warp>().AllowWarp(true);
							tmp.GetComponent<Warp>().setWarpMate(unPairedWarp);
							tmp.GetComponent<Warp>().AllowWarp(true);
							danglingWarp = false;
							unPairedWarp = null;
						}
						tmp.transform.parent = m_warpContainer;
						break;
					// Walls 
					case 'x': // horozantal wall 90 deg rotation
						tmp = Instantiate(wall, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));
						tmp.name = "wall";
						tmp.transform.parent = m_wallContainer;
						break; 
					case 'y': // vertical wall no rotation 
						tmp = Instantiate(wall, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 0, 0)));
						tmp.name = "wall";
						tmp.transform.parent = m_wallContainer;
						break; 
					case 'z': // doors to ghost house
						tmp = Instantiate(houseDoor, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));
						tmp.name = "door";
						tmp.transform.parent = m_wallContainer;
						break; 
					case '.':
						tmp = Instantiate(dot, new Vector3(x, 0, z), Quaternion.identity);
						tmp.name = "dot";
						tmp.transform.parent = m_dotContainer;					
						m_dCount++;
						break;
					case '*':
						tmp = Instantiate(energizer, new Vector3(x, 0, z), Quaternion.identity);
						tmp.name = "energizer";
						tmp.transform.parent = m_energizerContainer;
						m_eCount++;
						break;
					// Ghosts 
					case 'B':
						tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity);
						GhostController blinkyTmp = tmp.GetComponent<GhostController>();
						blinkyTmp.persona = GhostName.Blinky;
						blinkyTmp.setLevel(this); 
						tmp.name = "Blinky";
						m_ghosts.Add(tmp);
						tmp.transform.parent = m_ghostContainer;
						m_blinky = tmp.transform;
						break;
					case 'P':
						tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity);
						GhostController pinkyTmp = tmp.GetComponent<GhostController>();
						pinkyTmp.persona = GhostName.Pinky;
						pinkyTmp.setLevel(this);
						tmp.name = "Pinky";
						m_ghosts.Add(tmp);
						tmp.transform.parent = m_ghostContainer;
						m_pinky = tmp.transform;
						break;
					case 'I':
						tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity);
						GhostController inkyTmp = tmp.GetComponent<GhostController>();
						inkyTmp.persona = GhostName.Inky;
						inkyTmp.setLevel(this);
						tmp.name = "Inky";
						m_ghosts.Add(tmp);
						tmp.transform.parent = m_ghostContainer;
						m_inky = tmp.transform;
						break;
					case 'C':
						tmp = Instantiate(ghost, new Vector3(x, 0, z), Quaternion.identity);
						GhostController clydeTmp = tmp.GetComponent<GhostController>();
						clydeTmp.persona = GhostName.Clyde;
						clydeTmp.setLevel(this);
						tmp.name = "Clyde";
						m_ghosts.Add(tmp);
						tmp.transform.parent = m_ghostContainer;
						m_clyde = tmp.transform;
						break;
					// Player 
					case 's':
						tmp = Instantiate(player, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, -90, 0)));
						tmp.name = "player";
						m_pacMan = tmp.transform;
						tmp.GetComponent<Player>().Init();
						foreach(GameObject ghost in m_ghosts) {
							ghost.GetComponent<GhostController>().setPacMan(tmp.GetComponent<Player>());
							ghost.GetComponent<Pathfinding>().setTarget(m_pacMan.transform);
							ghost.GetComponent<Pathfinding>().setLevel(this);
							tmp.GetComponent<Player>().AddGhost(ghost);
						}
						Camera.main.GetComponent<SmoothCam>().target = tmp.transform;
						break;
				}

				bool walkable = !(Physics.CheckSphere(new Vector3(x, 0, z), .4f ,unwalkableMask));
				m_grid[z, x] = new Node(walkable, new Vector3(x, 0, z), z, x);
			}
		}
		m_levelBuilt = true;

		// set the availible dot count so we know when we've cleared the board (win condition)
		player.GetComponent<Player>().m_dotsRemain = m_dCount;
	}

	// for pathfinding
	public Node NodeFromWorldPoint(Vector3 worldPosition) {
		int x = Mathf.RoundToInt(worldPosition.x);
		int z = Mathf.RoundToInt(worldPosition.z);

		return m_grid[z, x];
	}

	// for pathfinding
	public List<Node> GetNeighbours(Node node) {
		List<Node> neighbours = new List<Node>(); 

		for(int z = 1; z >= -1; z--) {
			for(int x = -1; x <= 1; x++) {
				if(x == 0 && z == 0) {
					continue; // TODO: cleanup
				}
				
				int checkX = node.m_gridX + x; 
				int checkZ = node.m_gridZ + z; 
				
				if(checkX >= 0 && checkX < m_levelWidth && checkZ >=0 && checkZ < m_levelDepth) {
					neighbours.Add(m_grid[checkZ, checkX]); 
				}
			}
		}
		return neighbours;
	}

	public Transform getPacMan() { return m_pacMan.transform; }
	public Node[,] getGrid() { return m_grid; }

	// debugging in scene view 
	 void OnDrawGizmos() {

        if(m_grid != null && showPathfindingGrid) {  

			Node blinkyNode = NodeFromWorldPoint(m_blinky.position);
			Node pinkyNode = NodeFromWorldPoint(m_pinky.position);
			Node inkyNode = NodeFromWorldPoint(m_inky.position);
			Node clydeNode = NodeFromWorldPoint(m_clyde.position);
			
            foreach( Node n in m_grid) {
                Gizmos.color = (n.m_walkable) ? Color.white : Color.red;                
    
				if(blinkyNode == n) { continue; } 
				if(pinkyNode == n) {continue; }
				if(inkyNode == n) {continue; }
				if(clydeNode == n) {continue; }

				if (wireframe) {
					Gizmos.DrawWireCube(n.m_worldPos, Vector3.one * (m_nodeDiameter - .1f));
				} else {
					Gizmos.DrawCube(n.m_worldPos, Vector3.one * (m_nodeDiameter - .1f));
				}
            }
        }
    }
}
