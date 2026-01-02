using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private static bool isGameStarted = false;

    [Header("--- 패널 연결 ---")]
    public GameObject startPanel;
    public GameObject successPanel;
    public GameObject failMoneyPanel;
    public GameObject failStressPanel;

    public GameObject bankPanel;
    public GameObject storePanel;
    public GameObject academyPanel;
    public GameObject reportPanel;
    public GameObject nextPanel;
    public GameObject alertPanel;

    [Header("--- 효과 및 연출 ---")]
    public Image fadeImage;

    [Header("--- 상단 HUD ---")]
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI savingsText;
    public TextMeshProUGUI loanText;
    public TextMeshProUGUI stressText;
    public TextMeshProUGUI salaryText;

    [Header("--- 명세서 텍스트 ---")]
    public TextMeshProUGUI txtReportSalary;
    public TextMeshProUGUI txtReportPension;
    public TextMeshProUGUI txtReportHealth;
    public TextMeshProUGUI txtReportTax;
    public TextMeshProUGUI txtReportLoan;
    public TextMeshProUGUI txtReportNetPay;
    public TextMeshProUGUI txtReportLiving;
    public TextMeshProUGUI txtReportSavings;
    public TextMeshProUGUI txtReportS_Interest;
    public TextMeshProUGUI txtReportFinal;

    [Header("--- 결과 및 알림 텍스트 ---")]
    public TextMeshProUGUI txtResultScore;
    public TextMeshProUGUI txtAlertMsg;

    [Header("--- 버튼 ---")]
    public Button savingsJoinBtn;
    public TextMeshProUGUI savingsBtnText;

    [Header("--- 게임 데이터 ---")]
    public int currentMonth = 1;
    public long cash = 300000;
    public long savings = 0;
    public long loan = 0;
    public int stress = 0;
    public int jobLevel = 1;
    public int baseSalary = 2000000;

    // [밸런스 수정] 생활비 100만원으로 인상
    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 1000000;
    private bool isSavingsJoined = false;

    // [기능 추가] 월 1회 제한 체크용 변수
    private bool hasStudiedThisMonth = false;
    private bool hasPromotedThisMonth = false;

    void Awake()
    {
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        CloseAllPanels();
        UpdateUI();

        if (!isGameStarted)
        {
            Time.timeScale = 0;
            if (startPanel) startPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            if (startPanel) startPanel.SetActive(false);
        }
    }

    public void GameStart()
    {
        isGameStarted = true;
        Time.timeScale = 1;
        if (startPanel) startPanel.SetActive(false);
        UpdateUI();
    }

    public void GameExit()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // [기능 1] 메인 메뉴로 돌아가기 (기존 GameRestart)
    // -> Start_Panel이 뜹니다.
    public void GameRestart()
    {
        isGameStarted = false; // "게임 시작 전" 상태로 설정
        ResetGameData();       // 데이터 초기화
        SceneManager.LoadScene("MainScene");
    }

    // [기능 2] 바로 다시 시작하기 (NEW)
    // -> Start_Panel 없이 바로 1개월차 시작!
    public void GameRetry()
    {
        isGameStarted = true;  // "게임 시작됨" 상태로 설정
        ResetGameData();       // 데이터 초기화
        SceneManager.LoadScene("MainScene");
    }

    // (공통) 데이터 초기화 함수 (코드를 깔끔하게 하기 위해 분리)
    void ResetGameData()
    {
        Time.timeScale = 1;
        currentMonth = 1;
        cash = 300000;
        stress = 0;
        savings = 0;
        loan = 0;
        jobLevel = 1;
        baseSalary = 2000000;
        isSavingsJoined = false;
        hasStudiedThisMonth = false;
        hasPromotedThisMonth = false;
    }

    public void OnClickNextMonth()
    {
        if (currentMonth > 10) return;

        CloseAllPanels();

        CalculateAndShowReport();

        if (reportPanel) reportPanel.SetActive(true);

        Time.timeScale = 0;
    }

    void CalculateAndShowReport()
    {
        int currentSalary = baseSalary;

        int pension = (int)(currentSalary * 0.045f);
        int health = (int)(currentSalary * 0.035f);
        int tax = (int)(currentSalary * 0.03f);

        int totalDeduction = pension + health + tax;
        int netPay = currentSalary - totalDeduction;

        int loanInterest = 0;
        if (loan > 0)
        {
            loanInterest = (int)(loan * 0.02f);
            netPay -= loanInterest;
        }

        bool isSavingsSuccess = false;
        if (isSavingsJoined)
        {
            if (cash + netPay >= MONTHLY_SAVINGS_AMOUNT)
            {
                savings += MONTHLY_SAVINGS_AMOUNT;
                netPay -= MONTHLY_SAVINGS_AMOUNT;
                isSavingsSuccess = true;
            }
        }

        int interest = (int)(savings * 0.005f);
        savings += interest;

        cash += netPay;
        cash -= LIVING_COST;

        // [밸런스] 다음 달 스트레스 +60
        stress += 60;

        if (txtReportSalary) txtReportSalary.text = $"{currentSalary:N0}";
        if (txtReportPension) txtReportPension.text = $"-{pension:N0}";
        if (txtReportHealth) txtReportHealth.text = $"-{health:N0}";
        if (txtReportTax) txtReportTax.text = $"-{tax:N0}";
        if (txtReportLoan) txtReportLoan.text = (loan > 0) ? $"-{loanInterest:N0}" : "0";
        if (txtReportNetPay) txtReportNetPay.text = $"{netPay:N0}";
        if (txtReportLiving) txtReportLiving.text = $"-{LIVING_COST:N0}";

        if (txtReportSavings)
        {
            if (!isSavingsJoined) txtReportSavings.text = "미가입";
            else txtReportSavings.text = isSavingsSuccess ? $"-{MONTHLY_SAVINGS_AMOUNT:N0}" : "잔액 부족";
        }
        if (txtReportS_Interest) txtReportS_Interest.text = $"+{interest:N0}";

        int finalChange = netPay - LIVING_COST;
        if (txtReportFinal) txtReportFinal.text = $"{finalChange:N0}";
    }

    public void OnConfirmReport()
    {
        if (reportPanel) reportPanel.SetActive(false);
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        yield return new WaitForSecondsRealtime(0.5f);

        // 날짜 변경 및 초기화
        currentMonth++;

        // [중요] 다음 달이 되었으니 행동 제한 초기화!
        hasStudiedThisMonth = false;
        hasPromotedThisMonth = false;

        if (currentMonth > 10)
        {
            if (fadeImage != null)
            {
                float t = 1;
                while (t > 0)
                {
                    t -= Time.unscaledDeltaTime * 2f;
                    fadeImage.color = new Color(0, 0, 0, t);
                    yield return null;
                }
                fadeImage.color = new Color(0, 0, 0, 0);
                fadeImage.gameObject.SetActive(false);
            }
            EndGame("완료");
            yield break;
        }

        if (stress >= 100) EndGame("스트레스");
        else if (cash < 0) EndGame("파산");

        UpdateUI();

        if (fadeImage != null)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 0);
            fadeImage.gameObject.SetActive(false);
        }

        Time.timeScale = 1;
    }

    public void ShowAlert(string msg)
    {
        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = msg;
        }
    }

    public void CloseAlert()
    {
        if (alertPanel) alertPanel.SetActive(false);
    }

    // --- [기능] 실무 참여 (월 1회 제한 + 밸런스 조정) ---
    public void ActionPromotion()
    {
        // 1. 횟수 제한 체크
        if (hasPromotedThisMonth)
        {
            ShowAlert("이번 달은 더 이상 참여할 수 없습니다.\n(월 1회 제한)");
            return;
        }

        if (cash < 500000) { ShowAlert("참가비(50만원)가 부족합니다!"); return; }
        if (stress > 60) { ShowAlert("스트레스가 너무 높습니다!\n(60 이하일 때 가능)"); return; }

        cash -= 500000;
        baseSalary += 500000;

        // [밸런스] 스트레스 대폭 증가
        stress += 40;
        if (stress > 100) stress = 100;

        // 실행 완료 표시
        hasPromotedThisMonth = true;

        ShowAlert("실무 참여 완료!\n월급이 인상되었습니다.");
        CloseAllPanels();
        UpdateUI();
    }

    // --- [기능] 공부하기 (월 1회 제한) ---
    public void ActionStudy()
    {
        // 1. 횟수 제한 체크
        if (hasStudiedThisMonth)
        {
            ShowAlert("이번 달 공부는 이미 마쳤습니다.\n(월 1회 제한)");
            return;
        }

        if (cash < 100000) { ShowAlert("수강료가 부족합니다."); return; }

        cash -= 100000;
        jobLevel++;
        stress += 10;
        if (stress > 100) stress = 100;

        // 실행 완료 표시
        hasStudiedThisMonth = true;

        UpdateUI();
        ShowAlert("직무 능력 향상!\n(월급 인상 기대 가능)");
        CloseAllPanels(); // 학원 창 닫기
    }

    public void ActionBorrow()
    {
        if (loan >= 2000000) { ShowAlert("대출 한도 초과!"); return; }
        loan += 500000; cash += 500000; UpdateUI();
    }

    public void ActionRepay()
    {
        if (loan <= 0) { ShowAlert("상환할 대출이 없습니다."); return; }
        if (cash < 500000) { ShowAlert("잔액이 부족합니다."); return; }
        loan -= 500000; cash -= 500000; UpdateUI();
    }

    public void ActionJoinSavings()
    {
        if (isSavingsJoined) return;
        isSavingsJoined = true; UpdateUI();
    }

    public void ActionBuy()
    {
        if (cash < 50000) { ShowAlert("돈이 부족합니다."); return; }

        // [밸런스] 쇼핑 효율 감소 (-30 -> -20)
        cash -= 50000;
        stress -= 20;
        if (stress < 0) stress = 0;

        UpdateUI();
    }

    public void ActionRest()
    {
        // [밸런스] 휴식 효율 증가 (-10 -> -20)
        stress -= 20;
        if (stress < 0) stress = 0;

        UpdateUI();
    }

    public void OnCarAccident()
    {
        int penalty = 50000;
        if (cash >= penalty) cash -= penalty; else cash = 0;
        stress += 10; if (stress > 100) stress = 100;
        UpdateUI(); ShowAlert($"교통사고! 치료비 -{penalty:N0}원\n스트레스 +10");
    }

    public void CloseAllPanels()
    {
        if (bankPanel) bankPanel.SetActive(false);
        if (storePanel) storePanel.SetActive(false);
        if (academyPanel) academyPanel.SetActive(false);
        if (reportPanel) reportPanel.SetActive(false);
        if (nextPanel) nextPanel.SetActive(false);
        if (alertPanel) alertPanel.SetActive(false);

        if (successPanel) successPanel.SetActive(false);
        if (failMoneyPanel) failMoneyPanel.SetActive(false);
        if (failStressPanel) failStressPanel.SetActive(false);
    }

    void UpdateUI()
    {
        if (monthText) monthText.text = $"{currentMonth}개월차";
        if (cashText) cashText.text = $"{cash:N0}";
        if (savingsText) savingsText.text = $"{savings:N0}";
        if (loanText) loanText.text = $"{loan:N0}";
        if (stressText)
        {
            stressText.text = $"{stress}%";
            stressText.color = stress > 80 ? Color.red : Color.white;
        }
        if (salaryText) salaryText.text = $"{baseSalary:N0}";

        if (isSavingsJoined && savingsBtnText != null) savingsBtnText.text = "가입 완료";
        if (isSavingsJoined && savingsJoinBtn != null) savingsJoinBtn.interactable = false;
    }

    void EndGame(string type)
    {
        Time.timeScale = 0;
        CloseAllPanels();
        long finalScore = cash + savings - loan;

        if (type == "스트레스") if (failStressPanel) failStressPanel.SetActive(true);
            else if (type == "파산") if (failMoneyPanel) failMoneyPanel.SetActive(true);
                else
                {
                    if (successPanel) successPanel.SetActive(true);
                    if (txtResultScore != null) txtResultScore.text = $"TOTAL SCORE: {finalScore:N0}";
                }
    }
}