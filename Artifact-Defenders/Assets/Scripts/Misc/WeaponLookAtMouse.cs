using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quay vũ khí theo hướng di chuyển của nhân vật, dùng với PlayerSlash
/// </summary>
public class WeaponLookAtMovement : MonoBehaviour
{
    private SpriteRenderer sprite;
    private PlayerMovement playerMovement;

    void Start()
    {
        sprite = GetComponentInChildren<SpriteRenderer>();
        playerMovement = GetComponentInParent<PlayerMovement>();
    }

    private void Update()
    {
        if (playerMovement == null) return;

        Vector3 dir = playerMovement.MoveDirection;

        // Nếu không di chuyển thì giữ nguyên hướng cũ
        if (dir.magnitude < 0.1f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // Lật sprite theo hướng
        if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 180)
        {
            sprite.flipY = true;    
        }
        else
        {
            sprite.flipY = false;
        }
    }
}

//cơ chế cũ đánh bằng chuột chạm màn hình và đánh theo hướng chuột
//﻿using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
/////  Xoay đối tượng theo vị trí của chuột, được sử dụng với <xem cref="PlayerSlash"/> để tấn công cận chiến
///// </summary>
//public class WeaponLookAtMouse : MonoBehaviour
//{
//    Vector3 mousePos;
//    private SpriteRenderer sprite;

//    void Start()
//    {
//        sprite = GetComponentInChildren<SpriteRenderer>();
//    }

//    private void Update()
//    {
//        var dir = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
//        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
//        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

//        if (transform.rotation.eulerAngles.z > 0 && transform.rotation.eulerAngles.z < 180)
//        {
//            sprite.flipY = true;
//        }
//        else
//        {
//            sprite.flipY = false;
//        }
//    }
//}