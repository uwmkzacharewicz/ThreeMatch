using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dot : MonoBehaviour
{
    public static Dot selectedDot;

    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private FindMatches findMatches;
    private HintManager hintManager;
    private EndGameManager endGameManager;

    // Dodajemy now¹ zmienn¹ dla powiêkszonej skali
    public Vector3 enlargedScale = new Vector3(1.3f, 1.3f, 1.3f);
    private Vector3 originalScale;

    public GameObject otherDot;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    private Board board;


    [Header("Swipe Stuff")]
    public float swipeAngle = 0;

    [Header("Powerup Stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject adjacentBomb;


    private Vector3 initialPosition;

    // dodajemy zmienn¹ do przechowywania pozycji s¹siada
    private Vector3 neighborPosition;

    // Start is called before the first frame update
    void Start()
    {

        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        hintManager = FindObjectOfType<HintManager>();
        endGameManager = FindObjectOfType<EndGameManager>();
      
        // Zapisz oryginaln¹ skalê
        originalScale = transform.localScale;



    }

    // This is for testing and Debug only
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }

    void Update()
    {

        //if(isMatched)
        //{
        //    SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        //    mySprite.color = new Color(0f, 0f, 0f, .2f);
        //}

        targetX = column;
        targetY = row;

        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move towards the target
            Vector2 tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)            
                board.allDots[column, row] = this.gameObject;
            findMatches.FindAllMatches();
        }
        else if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move towards the target
            Vector2 tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .4f);
            if (board.allDots[column, row] != this.gameObject)
                board.allDots[column, row] = this.gameObject;
            findMatches.FindAllMatches();
        }
        else
        {
            //Directly set the position
            Vector2 tempPosition = new Vector2(targetX, targetY);
            transform.position = tempPosition;
        }
    }

    // Dodajemy metodê do ustawiania s¹siada
    public void SetNeighborPosition(Vector3 neighborPos)
    {
        neighborPosition = neighborPos;
    }

    
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            //Generowanie wskazówki
            if (hintManager != null)
                hintManager.DestroyHint();


            if (selectedDot == null) // if there is no dot selected
            {
                // assign the current dot as the selected one
                selectedDot = this;
                Enlarge();

                
            }
            else // if there is a selected dot
            {
                if (selectedDot == this) // if this dot was already selected
                {
                    ResetScale(); // reset the scale of this dot
                    selectedDot = null; // remove the reference to the selected dot
                }
                else // if this is a new dot
                {
                    // Check if this dot is a neighbor of the selected dot
                    if (IsNeighborOfSelectedDot())
                    {
                        SwapPieces(selectedDot);
                        selectedDot.ResetScale(); // reset the scale of the previously selected dot
                        selectedDot = null; // remove the reference to the selected dot
                    }
                    else // if this dot is not a neighbor of the selected dot
                    {
                        selectedDot.ResetScale(); // reset the scale of the previously selected dot
                        selectedDot = this; // assign the current dot as the selected one
                        Enlarge();
                    }
                }
            }

        }

        


    }

    bool IsNeighborOfSelectedDot()
    {
        // Check if this dot is a neighbor of the selected dot
        return (Mathf.Abs(selectedDot.column - column) == 1 && selectedDot.row == row) ||
               (Mathf.Abs(selectedDot.row - row) == 1 && selectedDot.column == column);
    }

    void Enlarge()
    {
        // Enlarge the dot
        transform.localScale = enlargedScale;
    }

    void ResetScale()
    {
        // Reset the scale to the original size
        transform.localScale = originalScale;
    }

    void SwapPieces(Dot otherDot)
    {
        if (otherDot != null)
        {
            // Keep track of previous columns and rows
            previousColumn = column;
            previousRow = row;
            otherDot.previousColumn = otherDot.column;
            otherDot.previousRow = otherDot.row;

            // Swap in the allDots array
            GameObject temp = board.allDots[otherDot.column, otherDot.row];
            board.allDots[otherDot.column, otherDot.row] = board.allDots[column, row];
            board.allDots[column, row] = temp;

            // Swap column and row values
            int tempColumn = otherDot.column;
            int tempRow = otherDot.row;
            otherDot.column = column;
            otherDot.row = row;
            column = tempColumn;
            row = tempRow;

            this.otherDot = otherDot.gameObject; // Update the otherDot variable

            board.currentDot = this; // Update the currentDot variable
        }

       
        StartCoroutine(CheckMoveCo());

        // destroy hint

    }


    public IEnumerator CheckMoveCo()
    {
        board.currentState = GameState.wait; // Dodajemy to na pocz¹tku koreutyny

        if(isColorBomb)
        {
            // This piece is a color bomb, the other piece is the color to destroy
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }
        else if(otherDot.GetComponent<Dot>().isColorBomb)
        {
            // The other piece is a color bomb, this piece has the color to destroy
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }

        yield return new WaitForSeconds(.5f);

        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = otherDot.GetComponent<Dot>().previousRow;
                otherDot.GetComponent<Dot>().column = otherDot.GetComponent<Dot>().previousColumn;
                row = previousRow;
                column = previousColumn;
                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move; // Wracamy do stanu move, jeœli nie ma dopasowania
            }
            else
            {
                if(endGameManager != null)
                {
                    if (endGameManager.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }
                board.DestroyMatches();     
                //board.currentState = GameState.move;
            }
            //otherDot = null;
        }

    }

    public void MakeRowBomb()
    {
        if (!isColumnBomb && !isRowBomb)
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void MakeColumnBomb()
    {
        if (!isColumnBomb && !isRowBomb)
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    public void MakeColorBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb)
        {
            isColorBomb = true;
            GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
            color.transform.parent = this.transform;
            this.gameObject.tag = "Color";
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isColorBomb)
        {
            isAdjacentBomb = true;
            GameObject adjacent = Instantiate(adjacentBomb, transform.position, Quaternion.identity);
            adjacent.transform.parent = this.transform;
        }
    }


}