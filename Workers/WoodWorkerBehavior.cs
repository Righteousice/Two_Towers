using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class WoodWorkerBehavior : MonoBehaviour
{
    public bool canMove;
    NavMeshAgent workerAgent;
    private GameObject tower;
    private BoxCollider woodResource;
    private Renderer woodResourceEmission;
    private bool atWoodResource;
    private PlayStopWoodWorker playStop;
    public Canvas playStopWood;
    public int woodAmountWorker;
    private Vector3 startPos;

    private void Awake()
    {
        ResourceBehavior.Init();
    }

    void Start()
    {
        workerAgent = GetComponent<NavMeshAgent>();
        tower = GameObject.Find("Player_Tower");
        woodResource = GameObject.Find("Wood").GetComponent<BoxCollider>();
        woodResourceEmission = GameObject.Find("Wood").GetComponent<Renderer>();
        woodResourceEmission.material.DisableKeyword("_EMISSION");
        playStop = GameObject.Find("Play_Button_Wood").GetComponent<PlayStopWoodWorker>();
        GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        onResourceClick();
        CanWorkerMove();
        onWorkerClick();
    }

    private void onResourceClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "WoodResource")
                {
                    workerAgent.destination = hit.point;
                }
            }
        }
    }

    private void onWorkerClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            int layerMask = LayerMask.GetMask("Worker");
            if (Physics.Raycast(ray, out hit, layerMask))
            {
                if (hit.collider.tag == "WoodWorker")
                {
                    playStopWood.GetComponent<Canvas>().enabled = true;
                    GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    woodResourceEmission.material.EnableKeyword("_EMISSION");
                    Debug.Log("Wood Worker selected");
                }
                else if (hit.collider.tag != "WoodWorker")
                {
                    playStopWood.enabled = false;
                    GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    woodResourceEmission.material.DisableKeyword("_EMISSION");
                    Debug.Log("Worker deselected");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider == woodResource)
        {
            atWoodResource = true;
            woodAmountWorker++;
            StartCoroutine(WorkerWoodResourceWaitTime());
        }
        if (collider.gameObject.CompareTag("Tower") && canMove && atWoodResource)
        {
            atWoodResource = false;
            Debug.Log("Worker has returned 1 Wood resource to tower");
            StartCoroutine(WorkerWoodResourceWaitTime());
        }
    }

    IEnumerator WorkerWoodResourceWaitTime()
    {
        yield return new WaitForSeconds(1.5f);
        if (atWoodResource)
        {
            workerAgent.destination = tower.transform.position;
        }
        else if (!atWoodResource)
        {
            workerAgent.destination = woodResource.transform.position;
        }
    }

    private bool CanWorkerMove()
    {
        if (playStop.stopped)
        {
            if (woodAmountWorker > 0)
            {
                workerAgent.destination = tower.transform.position;
            }
            else
            {
                workerAgent.destination = startPos;
            }
            if (workerAgent.transform.position == startPos)
            {
                canMove = false;
                workerAgent.speed = 0;
            }
        }
        else 
        {
            canMove = true;
        }
        return canMove;
    }
}
