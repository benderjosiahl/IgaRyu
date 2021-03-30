using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SoundDetectionBehavior : MonoBehaviour
{
    /** 
     * a static list of all agents using this component. This makes it super easy to alert all agents
     * in the scene without needing a variable for all agents. Whenever something makes a "sound", you 
     * simply need to call SoundDetectionBehavior.AlertAgentsToSound() and give it the vector location
     * of the sound and the loudness of the sound. The class will handle alerting the individial agents
     * because they are all stored here
     * 
     * See AlertAgentsToSound() ----- Line 126
     */
    public static List<SoundDetectionBehavior> agents = new List<SoundDetectionBehavior>();

    public enum AlertLevel
    {
        HIGH,
        MED,
        LOW
    }

    // components
    private static GameObject player;
    private Rigidbody rb;
    private NavMeshAgent navAgent;

    // how aware the AI is of the player
    [Range(0, 100)] private float currentDetectionLevel; //<------------------ dont ask me if this range decorator works even tho it's private, i have no clue.
    private const float MAX_DETECTION_LEVEL = 100F;
    /// <summary>
    /// returs a value between 0 (unaware) and 1 (aware)
    /// </summary>
    public float CurrentDetectionLevel { get { return currentDetectionLevel * 0.01f; } }
    [Header("sound detection control variables")]
    [SerializeField] [Range(0, 10 )] private float detectionPerSecDecrease = 2f;
    [SerializeField] [Range(0, 10 )] private float soundDetectionSensitivity = 1;
    [SerializeField] [Range(0, 100)] private float raiseDetectionStateToHighThreshold = 80;
    [SerializeField] [Range(0, 100)] private float raiseDetectionStateToMedThreshold = 40;


    // the current state of the AI
    private AlertLevel aIState;
    [SerializeField] private AlertLevel initState = AlertLevel.LOW;
    /// <summary>
    /// returns HIGH, MED, or LOW
    /// </summary>
    public AlertLevel CurrentAiState { get { return aIState; } }

    private Vector3 currentDestination;

    void Awake()
    {
        // finds the player without having to assign it in editor (less buggy that way)
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            // if the player is not found, throw an exception that says that the player doesn't have the player tag (so the game designer knows exactly how to fix it)
            if (player == null)
            {
                throw new System.Exception("Could not find the player to assign to nav agent, be sure to give your player game object the 'Player' tag");
            }
        }
        // add this AI to the list of AI in scene
        agents.Add(this);
        InitAI();
    }

    // Update is called once per frame
    void Update()
    {
        // based off state, check what the agent should do
        switch (aIState)
        {
            case AlertLevel.HIGH:
                // TODO: put code here to tell the agent what to do every frame if it is in the HIGH alert state
                break;
            case AlertLevel.MED:
                // TODO: put code here to tell the agent what to do every frame if it is in the MEDIUM alert state
                break;
            case AlertLevel.LOW:
                // TODO: put code here to tell the agent what to do every frame if it is in the LOW alert state
                break;
        }

        // lower the detection level by a certain about so that it decreases over time
        ChangeAwareness(detectionPerSecDecrease * Time.deltaTime * -1);
    }

    /// <summary>
    /// changes the alert level of the agent
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(AlertLevel newState)
    {
        /**
         * Useful note: if you need to determine what to do based on what state the agent is currently in (e.g. the 
         * agent needs to go check out that noise if it's going from LOW --> MED, but needs to stop if going from
         * HIGH --> MED) use an if statement to check the current state
         */
        switch (newState)
        {
            case AlertLevel.HIGH:
                // TODO: put code here to tell the agent what to do when the agent enters the HIGH alert state
                break;
            case AlertLevel.MED:
                // TODO: put code here to tell the agent what to do when the agent enters the MEDIUM alert state
                break;
            case AlertLevel.LOW:
                // TODO: put code here to tell the agent what to do when the agent enters the LOW alert state
                break;
        }

        aIState = newState;
    }

    /// <summary>
    /// Alerts all game objects with the Sound detection behavior to the sound
    /// </summary>
    /// <param name="alertLocation"></param>
    /// <param name="amplitude"></param>
    public static void AlertAgentsToSound(Vector3 alertLocation, float amplitude)
    {
        foreach (SoundDetectionBehavior soundDetectionAgent in agents)
        {
            soundDetectionAgent.AlertAgentToSound(alertLocation, amplitude);
        }
    }

    /// <summary>
    /// resets all agents to their init state
    /// </summary>
    public static void ResetAgents()
    {
        foreach (SoundDetectionBehavior soundDetectionAgent in agents)
        {
            //reset AI
            soundDetectionAgent.currentDetectionLevel = 0;
            soundDetectionAgent.ChangeState(soundDetectionAgent.initState);
        }
    }

    /// <summary>
    /// Alerts the agent to the player's location, and set's the awareness state to HIGH
    /// </summary>
    public void AlertAgentToPlayer()
    {
        SetDestination(player.transform.position);
        aIState = AlertLevel.HIGH;
    }

    private void InitAI()
    {
        currentDetectionLevel = 0;
        // set the default AI state
        ChangeState(initState);
    }

    // if the sound is loud enough, and close enough, go looking for the sound
    private void AlertAgentToSound(Vector3 alertLocation, float loudness)
    {
        // calc distance to event
        float distanceToSound = Mathf.Abs(Vector3.Distance(transform.position, alertLocation));

        // calc the ratio of dist to loudness (the louder the sound, the further away you can hear it)
        float _loudnessToDistanceRatio = loudness / distanceToSound;
        print(_loudnessToDistanceRatio);

        // raise awareness
        ChangeAwareness(_loudnessToDistanceRatio * soundDetectionSensitivity);
    }

    // raises the level of awareness of the player
    private void ChangeAwareness(float amount)
    {
        // make sure the detection level stays between 0 and 100
        currentDetectionLevel = Mathf.Clamp(currentDetectionLevel + amount, 0, 100f);


        // if the detection level reaches a certain amount, change the state
        if (currentDetectionLevel >= raiseDetectionStateToHighThreshold && aIState != AlertLevel.HIGH)
        {
            ChangeState(AlertLevel.HIGH);
            SetDestination(player.transform.position);
        } else if (currentDetectionLevel >= raiseDetectionStateToMedThreshold && aIState != AlertLevel.MED)
        {
            ChangeState(AlertLevel.MED);
        } else if (aIState != AlertLevel.LOW)
        {
            ChangeState(AlertLevel.LOW);
        }
    }

    private void SetDestination(Vector3 destinationVec)
    {
        currentDestination = destinationVec;
        navAgent.SetDestination(destinationVec);
    }

    private void OnDestroy()
    {
        // if the agent is destroyed, we need to remove it from the list of agents
        agents.Remove(this);
    }
}
