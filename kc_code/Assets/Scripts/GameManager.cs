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

    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 1000000;
    private bool isSavingsJoined = false;

    private bool hasStudiedThisMonth = false;
    private bool hasPromotedThisMonth = false;

    void Awake()
    {
        instance = this;
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

    public void GameRestart()
    {
        isGameStarted = false;
        ResetGameData();
        SceneManager.LoadScene("MainScene");
    }

    public void GameRetry()
    {
        isGameStarted = true;
        ResetGameData();
        SceneManager.LoadScene("MainScene");
    }

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
        // 10개월이 이미 지났다면 작동 방지
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

        // 월 정기 스트레스 증가
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
        // 페이드 아웃 연출
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

        // 한 달이 지나감
        currentMonth++;
        hasStudiedThisMonth = false;
        hasPromotedThisMonth = false;

        // [중요] 10개월을 무사히 마치고 11개월차로 넘어가는 순간 승리 처리
        if (currentMonth > 10)
        {
            if (fadeImage != null)
            {
                fadeImage.color = new Color(0, 0, 0, 0);
                fadeImage.gameObject.SetActive(false);
            }
            EndGame("완료"); // 여기서 SuccessPanel이 활성화됨
            yield break;
        }

        // 중간 패배 조건 체크
        if (stress >= 100)
        {
            EndGame("스트레스");
            yield break;
        }
        else if (cash < 0)
        {
            EndGame("파산");
            yield break;
        }

        UpdateUI();

        // 페이드 인 연출
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

    public void ActionPromotion()
    {
        if (hasPromotedThisMonth)
        {
            ShowAlert("이번 달은 더 이상 참여할 수 없습니다.\n(월 1회 제한)");
            return;
        }

        if (cash < 500000) { ShowAlert("참가비(50만원)가 부족합니다!"); return; }
        if (stress > 60) { ShowAlert("스트레스가 너무 높습니다!\n(60 이하일 때 가능)"); return; }

        cash -= 500000;
        baseSalary += 500000;
        stress += 40;
        if (stress > 100) stress = 100;
        hasPromotedThisMonth = true;

        ShowAlert("실무 참여 완료!\n월급이 인상되었습니다.");
        CloseAllPanels();
        UpdateUI();
    }

    public void ActionStudy()
    {
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
        hasStudiedThisMonth = true;

        UpdateUI();
        ShowAlert("직무 능력 향상!\n(월급 인상 기대 가능)");
        CloseAllPanels();
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
        cash -= 50000;
        stress -= 20;
        if (stress < 0) stress = 0;
        UpdateUI();
    }

    public void ActionRest()
    {
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

        if (type == "스트레스")
        {
            if (failStressPanel) failStressPanel.SetActive(true);
        }
        else if (type == "파산")
        {
            if (failMoneyPanel) failMoneyPanel.SetActive(true);
        }
        else // type == "완료"
        {
            if (successPanel) successPanel.SetActive(true);
            if (txtResultScore != null) txtResultScore.text = $"TOTAL SCORE: {finalScore:N0}";
        }
    }
}