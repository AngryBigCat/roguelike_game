using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MoveObject
{
    public float restartLevelDelay = 1f;
    public int pointsPerFood = 10;
    public int pointsPerSuda = 20;
    public int wallDamage = 1;

    private Animator animator;
    private int food;
    private Text foodText;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        foodText.text = "Food: " + food;
        
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.playerTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if (horizontal != 0 || vertical != 0) {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("PlayerHit");
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameOver();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;

        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        if (Move(xDir, yDir, out hit)) {

        }

        CheckIfGameOver();

        GameManager.instance.playerTurn = false;
    }

    protected override void onCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;

        hitWall.DamageWall(wallDamage);

        animator.SetTrigger("PlayerChap");
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit") {
            Invoke("Restart", restartLevelDelay);
        }
        else if (other.tag == "Food") {
            food += pointsPerFood;
            other.gameObject.SetActive(false);
            foodText.text = "+" + pointsPerFood + " Food: " + food;
        }
        else if (other.tag == "Soda") {
            food += pointsPerSuda;
            other.gameObject.SetActive(false);
            foodText.text = "+" + pointsPerSuda + " Food: " + food;
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CheckIfGameOver()
    {
        if (food <= 0) {
            GameManager.instance.GameOver();
        }
    }

}
