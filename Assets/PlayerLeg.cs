using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class PlayerLeg : MonoBehaviour
{
    [SerializeField] Transform footTarget;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] PlayerLeg otherFoot = default;
    [SerializeField] Player player;
    public float speed = 1;
    public float stepDistance = 1;
    public float stepHeight = 0.5f;
    public bool dominantLeg;
    
    Vector3 legPosition, newPosition, highlightedNewPosition, oldPosition, savedPosition;
    public float lerp;
    public float lerp2;

    // Start is called before the first frame update
    void Start()
    {
        legPosition = transform.position;
        lerp = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!dominantLeg)
        {
            transform.position = legPosition;
        }

        if (player.GetMoveInput() == 0)
            StandingStill();
        else
            PlayerMove();

        
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(newPosition, 0.1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(highlightedNewPosition, 0.1f);
    }

    public bool IsMoving()
    {
        return lerp < 1;
    }

    public void PlayerMove()
    {
        Ray ray = new Ray(footTarget.position, Vector3.down);
        //Debug.DrawRay(footTarget.position, Vector3.down, Color.red);
        if (Physics.Raycast(ray, out RaycastHit info, 100, groundLayer.value))
        {
            highlightedNewPosition = info.point;
            if (Vector3.Distance(newPosition, info.point) > stepDistance && !otherFoot.IsMoving() && lerp >= 1)
            {
                lerp = 0;
                newPosition = info.point;
            }
        }

        if (lerp < 1)
        {
            Vector3 tempPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            tempPosition.y += Mathf.Sin(lerp * Mathf.PI) * stepHeight;

            legPosition = tempPosition;
            lerp += Time.deltaTime * speed;
        }
        else
        {
            oldPosition = newPosition;
        }
        
        savedPosition = transform.position;
        lerp2 = 0;
    }

    public void StandingStill()
    {
        lerp = 1;
        if (lerp2 < 1)
        {
            Vector3 playerPosition = player.GetComponent<Transform>().position;
            playerPosition = new Vector3(playerPosition.x - 0.2f, playerPosition.y, transform.position.z);
            Vector3 tempPosition = Vector3.Lerp(savedPosition, playerPosition, lerp2);
            //tempPosition.y += Mathf.Sin(lerp2 * Mathf.PI) * stepHeight/2;
            legPosition = tempPosition;
            lerp2 += Time.deltaTime * speed;
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }
}
