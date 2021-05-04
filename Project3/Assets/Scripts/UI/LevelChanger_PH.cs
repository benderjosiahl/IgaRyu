using UnityEngine;

public class LevelChanger_PH : MonoBehaviour
{
    public Animator animator;

    // Update is called once per frame
    void Update()
    {

    }

    private void OnFire()
    {
        FadeToLevel(1);
    }

    public void FadeToLevel (int levelIndex)
    {
        animator.SetTrigger("FadeOut");
    }
}
