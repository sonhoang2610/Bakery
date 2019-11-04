    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

[Serializable]
public class AnimationEventDelegate : UnityEvent<AnimationEvent>
{

}

public class AnimatorTrigger : MonoBehaviour {
    [SerializeField]
	AnimationEventDelegate action;
	[SerializeField]
	string[] eventStrings;
	[SerializeField]
	UnityEvent[] actions;
    public void trigger(AnimationEvent ev)
	{
		if(action != null){
			action.Invoke(ev);
		}
		int index = eventStrings.findIndex(ev.stringParameter);
		if(index < actions.Length){
			actions[index].Invoke();
		}
    }
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	[ContextMenu("JumpBoss")]
	public void JumpBoss(){
		GetComponent<Animator>().SetInteger("JumpState",1);
	}
	[ContextMenu("Laser")]
	public void laser(){
		GetComponent<Animator>().SetInteger("LaserState",1);
	}
	[ContextMenu("Rocket")]
	public void rocket(){
		GetComponent<Animator>().SetInteger("Rocket",1);
	}
}
