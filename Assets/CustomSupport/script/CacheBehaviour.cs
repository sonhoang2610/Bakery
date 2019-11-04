using UnityEngine;
using System;
//using Spine.Unity;
using UnityEngine.UI;
using UnityToolbag;
using UnityEngine.Events;

namespace UnityToolbag
{

    /// <summary>
    /// A safe, drop-in replacement for MonoBehaviour as your base class. Each property value is cached
    /// and GetComponent will be called if the instance is null or is destroyed.
    /// </summary>
    public  class CacheBehaviour : MonoBehaviour
    {
        [HideInInspector, NonSerialized]
        private SpriteMask _spriteMask;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public SpriteMask SpriteMask { get { return _spriteMask ? _spriteMask : (_spriteMask = GetComponent<SpriteMask>()); } }

        [HideInInspector, NonSerialized]
        private RootMotionController _rootMotion;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public  RootMotionController RootMotion { get { return _rootMotion ? _rootMotion : (_rootMotion = GetComponent<RootMotionController>()); } }

        [HideInInspector, NonSerialized]
        private Animation _animation;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public new Animation animation { get { return _animation ? _animation : (_animation = GetComponent<Animation>()); } }

        [HideInInspector, NonSerialized]
        private RectTransform _rectTranform;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public RectTransform RectTranform { get { return _rectTranform ? _rectTranform : (_rectTranform = GetComponent<RectTransform>()); } }

        [HideInInspector, NonSerialized]
        private ScheduleLayer _schedule;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public  ScheduleLayer Schedule { get { return _schedule ? _schedule : (_schedule = GetComponent<ScheduleLayer>()); } }


        [HideInInspector, NonSerialized]
        private Image _image;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public Image Image { get { return _image ? _image : (_image = GetComponent<Image>()); } }

        [HideInInspector, NonSerialized]
        private SpriteRenderer _spriteRenderer;

        /// <summary>
        /// Gets the Animation attached to the object.
        /// </summary>
        public  SpriteRenderer SpriteRenderer { get { return _spriteRenderer ? _spriteRenderer : (_spriteRenderer = GetComponent<SpriteRenderer>()); } }


        //[HideInInspector, NonSerialized]
        //private SkeletonAnimation _skeleton;


        /////// <summary>
        /////// Gets the Animation attached to the object.
        /////// </summary>
        //public SkeletonAnimation skeleton { get { return _skeleton ? _skeleton : (_skeleton = GetComponent<SkeletonAnimation>()); } }

        [HideInInspector, NonSerialized]
        private PhysicsMaterial2D _physicmaterial2D;


        ///// <summary>
        ///// Gets the Animation attached to the object.
        ///// </summary>
        public PhysicsMaterial2D PhysicMaterial2D { get { return _physicmaterial2D ? _physicmaterial2D : (_physicmaterial2D = GetComponent<PhysicsMaterial2D>()); } }

        [HideInInspector, NonSerialized]
        private AudioSource _audio;

        /// <summary>
        /// Gets the AudioSource attached to the object.
        /// </summary>
        public new AudioSource audio { get { return _audio ? _audio : (_audio = GetComponent<AudioSource>()); } }

        [HideInInspector, NonSerialized]
        private Camera _camera;

        /// <summary>
        /// Gets the Camera attached to the object.
        /// </summary>
        public new Camera camera { get { return _camera ? _camera : (_camera = GetComponent<Camera>()); } }

        [HideInInspector, NonSerialized]
        private Collider _collider;

        /// <summary>
        /// Gets the Collider attached to the object.
        /// </summary>
        public new Collider collider { get { return _collider ? _collider : (_collider = GetComponent<Collider>()); } }

        [HideInInspector, NonSerialized]
        private Collider2D _collider2D;

        /// <summary>
        /// Gets the Collider2D attached to the object.
        /// </summary>
        public new Collider2D collider2D { get { return _collider2D ? _collider2D : (_collider2D = GetComponent<Collider2D>()); } }

        [HideInInspector, NonSerialized]
        private ConstantForce _constantForce;

        /// <summary>
        /// Gets the ConstantForce attached to the object.
        /// </summary>
        public new ConstantForce constantForce { get { return _constantForce ? _constantForce : (_constantForce = GetComponent<ConstantForce>()); } }
        

        [HideInInspector, NonSerialized]
        private Text _textUI;

        /// <summary>
        /// Gets the GUIText attached to the object.
        /// </summary>
        public  Text TextUI { get { return _textUI ? _textUI : (_textUI = GetComponent<Text>()); } }



        [HideInInspector, NonSerialized]
        private HingeJoint _hingeJoint;

        /// <summary>
        /// Gets the HingeJoint attached to the object.
        /// </summary>
        public new HingeJoint hingeJoint { get { return _hingeJoint ? _hingeJoint : (_hingeJoint = GetComponent<HingeJoint>()); } }

        [HideInInspector, NonSerialized]
        private Light _light;

        /// <summary>
        /// Gets the Light attached to the object.
        /// </summary>
        public new Light light { get { return _light ? _light : (_light = GetComponent<Light>()); } }

        /*
		#if !UNITY_WEBGL
        [HideInInspector, NonSerialized]
        private NetworkView _networkView;

        /// <summary>
        /// Gets the NetworkView attached to the object.
        /// </summary>
        public new NetworkView networkView { get { return _networkView ? _networkView : (_networkView = GetComponent<NetworkView>()); } }
		#endif
        */

        [HideInInspector, NonSerialized]
        private ParticleSystem _particleSystem;

        /// <summary>
        /// Gets the ParticleSystem attached to the object.
        /// </summary>
        public new ParticleSystem particleSystem { get { return _particleSystem ? _particleSystem : (_particleSystem = GetComponent<ParticleSystem>()); } }

        [HideInInspector, NonSerialized]
        private Renderer _renderer;

        /// <summary>
        /// Gets the Renderer attached to the object.
        /// </summary>
        public new Renderer renderer { get { return _renderer ? _renderer : (_renderer = GetComponent<Renderer>()); } }

        [HideInInspector, NonSerialized]
        private Rigidbody _rigidbody;

        /// <summary>
        /// Gets the Rigidbody attached to the object.
        /// </summary>
        public new Rigidbody rigidbody { get { return _rigidbody ? _rigidbody : (_rigidbody = GetComponent<Rigidbody>()); } }

        [HideInInspector, NonSerialized]
        private Rigidbody2D _rigidbody2D;

        /// <summary>
        /// Gets the Rigidbody2D attached to the object.
        /// </summary>
        public new Rigidbody2D rigidbody2D { get { return _rigidbody2D ? _rigidbody2D : (_rigidbody2D = GetComponent<Rigidbody2D>()); } }



        [HideInInspector, NonSerialized]
        private Animator _animator;
        /// <summary>
        /// Get Animator controller
        /// </summary>
        public Animator animator { get { return _animator ? _animator : (_animator = GetComponent<Animator>()); } }


        public AnimtionEventCustom _eventAnimation;
        public void addAnimationEvent(UnityAction<AnimationEvent> pAction)
        {
            if(_eventAnimation == null)
            {
                _eventAnimation = new AnimtionEventCustom();
            }
            _eventAnimation.RemoveAllListeners();
            _eventAnimation.AddListener(pAction);
        }
        public void onAnimation(AnimationEvent pEvent)
        {
            if (_eventAnimation != null)
            {
                _eventAnimation.Invoke(pEvent);
            }
        }
    }

    public class AnimtionEventCustom : UnityEvent<AnimationEvent>
    {

    }
}
