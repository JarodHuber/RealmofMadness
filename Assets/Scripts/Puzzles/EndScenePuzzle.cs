using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScenePuzzle : MonoBehaviour
{
    public EndVO endVO;
    public Transform player;
    public UINumber counter;
    public GameObject blackScreen;
    public float duration = 30;

    Rigidbody playerRB;
    float timer = 0;
    bool moved = false, ending = false;

    private void Start()
    {
        playerRB = player.gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) >= 200.0f)
            player.position = transform.position;

        if(playerRB.velocity == Vector3.zero)
        {
            if (moved)
            {
                if (timer <= duration)
                    timer += Time.deltaTime;
                else if (!ending)
                    Win();
            }
        }
        else
        {
            timer = 0;
            moved = true;
        }

        if (ending && (endVO.voScript.audioSource.time/endVO.voScript.audioSource.clip.length) >= .4f)
        {
            //blackScreen.SetActive(true);
            SceneManager.LoadScene("CreditScene");
        }

        counter.Number((int)timer);
    }

    public void Win()
    {
        ending = true;
        endVO.voScript.CutOff(false);
    }
}
