using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaskTest
{
    public class GameRoot : MonoBehaviour, IPointerClickHandler
    {
        public static GameRoot Instance
        {
            get; set;
        }
        public Transform Target
        {
            get { return _rectMaskControl.Target; }
        }
        MaskControl _circleMaskControl;
        RectMaskControl _rectMaskControl;
        private MaskControl _MaskControl;
        Transform[] _targets;

        private void Switch()
        {
            if (_MaskControl == _rectMaskControl)
            {
                _rectMaskControl.gameObject.SetActive(false);
                _MaskControl = _circleMaskControl;
            }
            else
            {
                _circleMaskControl.gameObject.SetActive(false);
                _MaskControl = _rectMaskControl;
            }

            _MaskControl.gameObject.SetActive(true);
            _MaskControl.SetCurTarget();
            _targets = _MaskControl.targets;
        }


        private void Start()
        {
            _circleMaskControl = transform.Find("CircleMask").GetComponent<CircleMaskControl>();
            _rectMaskControl = transform.Find("RectMask").GetComponent<RectMaskControl>();
            Instance = this;
            Switch();

            for (int i = 0; i < _targets.Length; i++)
            {
                if (i != _targets.Length - 1)
                    _targets[i].GetComponent<Button>().onClick.AddListener(() =>
                    {
                        _MaskControl.CurTargetDone();
                        _MaskControl.SetCurTarget();
                    });
                else
                {
                    _targets[i].GetComponent<Button>().onClick.AddListener(() =>
                    {
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;

#else
                         Application.Quit();
#endif
                    });
                }
            }
        }

        void Update()
        {
            if(Input.GetKeyUp(KeyCode.A))
            {
                Switch();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PassEvent(eventData, ExecuteEvents.pointerClickHandler);
        }

        private void PassEvent<T>(PointerEventData pointerEventData, ExecuteEvents.EventFunction<T> eventFunction)
            where T : IEventSystemHandler
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, results);
            GameObject current = pointerEventData.pointerCurrentRaycast.gameObject;
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject == Target)
                {
                    ExecuteEvents.Execute(results[i].gameObject, pointerEventData, eventFunction);
                }
            }
        }
    }

}

