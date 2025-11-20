using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DafultScript
{
    public class EmptyHole : MonoBehaviour
    {
        [SerializeField] private Screw screw;

        public Screw Screw
        {
            get
            {
                return screw;
            }
            set
            {
                screw = value;
            }
        }
        /// <summary>
        /// 检查当前洞是否持有球
        /// </summary>
        public bool IsEmpty()
        {
            return screw == null;// && transform.childCount == 0;
        }

    }
}
