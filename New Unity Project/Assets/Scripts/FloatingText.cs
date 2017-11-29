using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour {
    public Animator animator;
    private Text damageText;

    private void OnEnable()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        damageText = animator.GetComponent<Text>();
        Destroy(gameObject, clipInfo[0].clip.length);
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }

}
