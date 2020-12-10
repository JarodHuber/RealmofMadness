using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public PuzzleGeneration puzzleManager;
    public Transform player;
    public AudioSource music;
    Collider playerCollider;
    SpriteRenderer sp;
    Vector3 portalBounds, playerBounds;
    public float distCheck = 1f;
    bool isPuzzle = false;
    string SceneToLoad;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
        playerCollider = player.GetComponent<Collider>();
        portalBounds = sp.bounds.extents;
        playerBounds = playerCollider.bounds.extents;
        SceneToLoad = "MainScene";
    }

    void Update()
    {
        if (player != puzzleManager.player.transform)
            player = puzzleManager.player.transform;

        if(player.position.x + playerBounds.x > transform.position.x - portalBounds.x && 
           player.position.x - playerBounds.x < transform.position.x + portalBounds.x)
        {
            if(Mathf.Abs(player.position.z - transform.position.z) < distCheck)
            {
                PlayerPrefs.SetFloat("songPoint", music.time);
                if(player.position.z < transform.position.z)
                {
                    if (puzzleManager.puzzleSolved)
                    {
                        PlayerPrefs.SetInt("puzzleSolved", 1);
                        puzzleManager.puzzleCompleted();
                        SceneManager.LoadScene(SceneToLoad);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("puzzleSolved", 0);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                else
                {
                    if (isPuzzle || puzzleManager.puzzleSolved)
                    {
                        PlayerPrefs.SetInt("puzzleSolved", 1);
                        puzzleManager.puzzleCompleted();
                        SceneManager.LoadScene(SceneToLoad);
                    }
                    else
                    {
                        PlayerPrefs.SetInt("puzzleSolved", 0);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
            }
        }
    }

    public void ActivatePuzzle(int puzzleType)
    {
        isPuzzle = true;
    }
}
