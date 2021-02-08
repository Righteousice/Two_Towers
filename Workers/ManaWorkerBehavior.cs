using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class ManaWorkerBehavior : MonoBehaviour
{
    public bool canMove;
    NavMeshAgent workerAgent;
    private GameObject tower;
    private BoxCollider manaResource;
    private Renderer manaResourceEmission;
    private bool atManaResource;
    private PlayStopManaWorker playStop;
    public Canvas playStopMana;
    public int manaAmountWorker;
    private Vector3 startPos;

    private void Awake()
    {
        ResourceBehavior.Init();
    }

    void Start()
    {
        workerAgent = GetComponent<NavMeshAgent>();
        tower = GameObject.Find("Player_Tower");
        manaResource = GameObject.Find("Mana").GetComponent<BoxCollider>();
        manaResourceEmission = GameObject.Find("Mana").GetComponent<Renderer>();
        manaResourceEmission.material.DisableKeyword("_EMISSION");
        playStop = GameObject.Find("Play_Button_Mana").GetComponent<PlayStopManaWorker>();
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
                if (hit.collider.tag == "ManaResource")
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
                if (hit.collider.tag == "ManaWorker")
                {
                    playStopMana.GetComponent<Canvas>().enabled = true;
                    GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
                    manaResourceEmission.material.EnableKeyword("_EMISSION");
                    Debug.Log("Worker selected");
                }
                else if (hit.collider.tag != "ManaWorker")
                {
                    playStopMana.GetComponent<Canvas>().enabled = false;
                    GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
                    manaResourceEmission.material.DisableKeyword("_EMISSION");
                    Debug.Log("Worker deselected");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider == manaResource)
        {
            atManaResource = true;
            manaAmountWorker++;
            StartCoroutine(WorkerManaResourceWaitTime());
        }
        if (collider.gameObject.CompareTag("Tower") && canMove)
        {
            atManaResource = false;
            Debug.Log("Worker has returned 1 Mana resource to tower");
            StartCoroutine(WorkerManaResourceWaitTime());
        }
    }

    IEnumerator WorkerManaResourceWaitTime()
    {
        yield return new WaitForSeconds(1.5f);
        if (atManaResource)
        {
            workerAgent.destination = tower.transform.position;
        }
        else if (!atManaResource)
        {
            workerAgent.destination = manaResource.transform.position;
        }
    }

    private bool CanWorkerMove()
    {
        if (playStop.stopped)
        {
            if (manaAmountWorker > 0)
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
