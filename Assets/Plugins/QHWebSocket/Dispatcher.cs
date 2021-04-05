using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dispatcher : MonoBehaviour {

	// Properties fields
	private static Dispatcher m_instance;
	private static bool 	  m_instance_exist = false;
	private static object 	  m_lock = new object();
	private static readonly Queue<Action> m_queue = new Queue<Action>();
	private static System.Threading.Thread m_mainThread;

	// Check this is mainThread or not
	public static bool isMainThread{
		get {
			return m_mainThread == System.Threading.Thread.CurrentThread;
		}
	}

	// Queue action to be invoke on main thread 
	public static void InvokeAsync(Action action){
		if (!m_instance_exist) {
			//Debug.LogErrorFormat ("No Dispatcher Exist in the scene. Action will not be invoked!");
			return;
		}
			
		if (isMainThread) {
			action.Invoke ();
		} else {
			lock (m_lock) {
				m_queue.Enqueue (action);
			}
		}
	}

	public static void Invoke(Action action){
		if (!m_instance_exist) {
			Debug.LogErrorFormat ("No Dispatcher Exist in the scene. Action will not be invoked!");
			return;
		}

		bool finish = false;
		InvokeAsync (() => {
			action.Invoke();
			finish = true;
		});

		while (!finish) {
			System.Threading.Thread.Sleep (5);
		}
	}
		
	void Awake(){
		if (m_instance != null) {
			DestroyImmediate (this);
		} else {
			m_instance = this;
			m_instance_exist = true;
			m_mainThread = System.Threading.Thread.CurrentThread;
		}
	}

	void OnDestroy(){
		if (m_instance == this) {
			m_instance = null;
			m_instance_exist = false;
		}
	}

	void Update(){
		
		// Call all action until action queue is empty
		lock (m_lock) {
			while (m_queue.Count > 0)
				m_queue.Dequeue () ();
		}
	}
}
