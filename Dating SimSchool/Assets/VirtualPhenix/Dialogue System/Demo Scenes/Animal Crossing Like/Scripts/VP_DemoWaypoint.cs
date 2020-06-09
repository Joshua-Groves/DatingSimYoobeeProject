using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog.Demo
{
    public class VP_DemoWaypoint : MonoBehaviour
    {
        [SerializeField] private float m_radius = 1;
        [SerializeField] private Color m_colorInGizmo = Color.blue;

        private void OnDrawGizmos()
        {
            Gizmos.color = m_colorInGizmo;
            Gizmos.DrawSphere(transform.position, m_radius);
        }
    }

}
