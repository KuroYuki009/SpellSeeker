using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneTransitionManager : MonoBehaviour
{
    InGameManager gameModeManager;

    //各所ステージ名。
    public List<SceneProfileDate> stageProfileDate;//出来たステージは各入れるようにする。
    public int loadStageNamber;

    // ロード中に表示するUI群。
    public GameObject loadingUI_Obj;
    LoadScreenEventProcess loadScreen_EP;
    Animator loadUI_Animator;

    // ロードの進捗状況を管理するための変数
    private AsyncOperation async;

    void Start()
    {
        loadScreen_EP = loadingUI_Obj.GetComponent<LoadScreenEventProcess>(); //ロード時のアニメーションUIの制御。
        loadUI_Animator = loadingUI_Obj.GetComponent<Animator>();//ロードUIのアニメーターコンを取得、格納。
        loadScreen_EP.sceneTransitionManager = GetComponent<SceneTransitionManager>();//このスクリプトのパスを渡す。
        gameModeManager = GetComponent<InGameManager>();
    }

    // ロードを開始するメソッド
    public void StartLoad()
    {
        // ロード画面を表示する
        loadingUI_Obj.SetActive(true);
        loadUI_Animator.SetTrigger("CutIn_Trigger");//アニメーションをカットインステートに移動。
    }

    public void StartingLoadProcess()//UIのアニメーションイベントから呼び出される。
    {
        Debug.Log("イベントが読み込まれた");
        StartCoroutine(Load());//コルーチンを使用してシーンを読み込む。
        loadUI_Animator.SetTrigger("LoadStandby_Trigger");//アニメーションを待機ステートに移動。
    }

    // コルーチンを使用してロードを実行するメソッド
    private IEnumerator Load()
    {
        yield return new WaitForSeconds(0.2f);//0.2秒待機。

        Debug.Log("ロード開始");
        // シーンを非同期でロードさせる
        async = SceneManager.LoadSceneAsync(stageProfileDate[loadStageNamber].sceneDateName);

        
        // ロードが完了するまで待機する
        while (!async.isDone)
        {
            yield return null;
        }
        Debug.Log("ロード終了");

        Resources.UnloadUnusedAssets();// メモリを解放する。

        EndLoad(); //ロード終了後の処理を行う。
    }

    public void EndLoad()//ロード終了後のメゾット。
    {
        loadUI_Animator.SetTrigger("CutOut_Trigger");//アニメーションをカットアウトステートに移動。
        //ここで外部へ信号を送り、現シーン名から要素を発生させる。
        gameModeManager.currentSceneDate = stageProfileDate[loadStageNamber];//ゲームモードマネージャに現在のシーン名を流す。 

        gameModeManager.SceneEntryProcess();//インゲームマネージャーに信号を送る。
    }

    public void EndLoadProcess()//ロード終了後の処理。
    {
        
    }

    public void Invalidation()//このオブジェクトの無効化。
    {
        // ロード画面を非表示にする
        loadingUI_Obj.SetActive(false);
    }
}
