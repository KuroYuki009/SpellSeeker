using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyAttack : MonoBehaviour
{
    public GameObject atkPrefab;//�U���Ɏg�p����v���o�t���i�[�B

    public Transform instPos;//�U���𔭐�������ʒu�B

    float atkDelayTime;//�U���̊��o���Ԃ̎w��B
    float elapsedTime;//�o�ߎ��Ԃ��w���B
    void Start()
    {
        atkDelayTime = 1;
    }

    void Update()
    {
        if(elapsedTime <= atkDelayTime)
        {
            elapsedTime += 1 * Time.deltaTime;
        }
        else
        {
            GameObject instPrefab = Instantiate(atkPrefab, instPos.transform.position, instPos.transform.rotation);//�J�[�h�f�[�^�����Ƀv���n�u�𐶐�����B

            instPrefab.GetComponent<SpellPrefabManager>().ownerObject = gameObject;//�X�y���v���n�u�}�l�[�W���[�ɃI�u�W�F�N�g����n���B

            elapsedTime = 0;
        }
    }
}
