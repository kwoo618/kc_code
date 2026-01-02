using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private static bool isGameStarted = false;

    [Header("--- 시작 및 종료 패널 ---")]
    public GameObject startPanel;
    public GameObject successPanel;
    public GameObject failMoneyPanel;
    public GameObject failStressPanel;

    [Header("--- 상단 HUD UI (TMP) ---")]
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI savingsText;
    public TextMeshProUGUI loanText;
    public TextMeshProUGUI stressText;
    public TextMeshProUGUI salaryText;

    [Header("--- 팝업 패널 리스트 ---")]
    public GameObject bankPanel;
    public GameObject storePanel;
    public GameObject academyPanel;
    public GameObject reportPanel;
    public GameObject nextPanel;

    [Header("--- 명세서 UI (각 항목별 연결) ---")]
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

    [Header("--- 결과 화면 점수 표시 ---")]
    public TextMeshProUGUI txtResultScore;

    [Header("--- 버튼 관련 ---")]
    public Button savingsJoinBtn;
    public TextMeshProUGUI savingsBtnText;

    [Header("--- 게임 데이터 ---")]
    public int currentMonth = 1;
    public long cash = 300000;
    public long savings = 0;
    public long loan = 0;
    public int stress = 0;
    public int jobLevel = 1;

    private const int BASE_SALARY = 2000000;
    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 500000;
    private bool isSavingsJoined = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CloseAllPanels();
        UpdateUI();

        if (!isGameStarted)
        {
            Time.timeScale = 0;
            if (startPanel != null) startPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            if (startPanel != null) startPanel.SetActive(false);
            if (failMoneyPanel) failMoneyPanel.SetActive(false);
            if (failStressPanel) failStressPanel.SetActive(false);
            if (successPanel) successPanel.SetActive(false);
        }
    }

    public void GameStart()
    {
        isGameStarted = true;
        Time.timeScale = 1;
        if (startPanel != null) startPanel.SetActive(false);
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
        Time.timeScale = 1;

        currentMonth = 1;
        cash = 300000;
        stress = 0;
        savings = 0;
        loan = 0;
        jobLevel = 1;
        isSavingsJoined = false;

        SceneManager.LoadScene("MainScene");
    }

    public void OnCarAccident()
    {
        int penalty = 50000;
        if (cash >= penalty) cash -= penalty;
        else cash = 0;

        stress += 10;
        if (stress > 100) stress = 100;

        UpdateUI();
    }

    public void OnClickNextMonth()
    {
        if (currentMonth >= 10) { EndGame("완료"); return; }

        int currentSalary = BASE_SALARY + ((jobLevel - 1) * 100000);

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

        stress += 20;
        if (stress > 100) stress = 100;

        // --- 명세서 UI ---
        if (txtReportSalary) txtReportSalary.text = $"{currentSalary:N0}";
        if (txtReportPension) txtReportPension.text = $"-{pension:N0}";
        if (txtReportHealth) txtReportHealth.text = $"-{health:N0}";
        if (txtReportTax) txtReportTax.text = $"-{tax:N0}";

        if (txtReportLoan)
        {
            if (loan > 0) txtReportLoan.text = $"-{loanInterest:N0}";
            else txtReportLoan.text = "0";
        }

        int realMoney = currentSalary - totalDeduction - loanInterest;
        if (txtReportNetPay) txtReportNetPay.text = $"{realMoney:N0}";

        if (txtReportLiving) txtReportLiving.text = $"-{LIVING_COST:N0}";

        if (txtReportSavings)
        {
            if (!isSavingsJoined) txtReportSavings.text = "미가입";
            else if (isSavingsSuccess) txtReportSavings.text = $"-{MONTHLY_SAVINGS_AMOUNT:N0}";
            else txtReportSavings.text = "잔액 부족";
        }

        if (txtReportS_Interest) txtReportS_Interest.text = $"+{interest:N0}";

        int finalChange = netPay - LIVING_COST;
        if (txtReportFinal) txtReportFinal.text = $"{finalChange:N0}";

        if (reportPanel) reportPanel.SetActive(true);

        currentMonth++;

        if (stress >= 100) EndGame("스트레스");
        else if (cash < 0) EndGame("파산");

        UpdateUI();
    }

    // --- 버튼 액션들 (수정됨: 패널 닫기 코드 삭제) ---
    public void ActionBorrow()
    {
        if (loan >= 2000000) return;
        loan += 500000; cash += 500000;
        UpdateUI();
        // bankPanel.SetActive(false); // 삭제됨: 이제 창이 안 닫힘
    }

    public void ActionRepay()
    {
        if (loan <= 0) return;
        if (cash < 500000) return;
        loan -= 500000; cash -= 500000;
        UpdateUI();
        // bankPanel.SetActive(false); // 삭제됨
    }

    public void ActionJoinSavings()
    {
        if (isSavingsJoined) return;
        isSavingsJoined = true;
        UpdateUI();
        // bankPanel.SetActive(false); // 삭제됨
    }

    public void ActionBuy()
    {
        if (cash < 50000) return;
        cash -= 50000; stress -= 30; if (stress < 0) stress = 0;
        UpdateUI();
        // storePanel.SetActive(false); // 삭제됨
    }

    public void ActionStudy()
    {
        if (cash < 100000) return;
        cash -= 100000; jobLevel++; stress += 10; if (stress > 100) stress = 100;
        UpdateUI();
        // academyPanel.SetActive(false); // 삭제됨
    }

    public void ActionRest()
    {
        stress -= 10; if (stress < 0) stress = 0;
        UpdateUI();
        // academyPanel.SetActive(false); // 삭제됨
    }

    // --- [중요] X 버튼에 연결할 함수 ---
    public void CloseAllPanels()
    {
        if (bankPanel) bankPanel.SetActive(false);
        if (storePanel) storePanel.SetActive(false);
        if (academyPanel) academyPanel.SetActive(false);
        if (reportPanel) reportPanel.SetActive(false);
        if (nextPanel) nextPanel.SetActive(false);

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

        int curSal = BASE_SALARY + ((jobLevel - 1) * 100000);
        if (salaryText) salaryText.text = $"{curSal:N0}";

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
        else
        {
            if (successPanel) successPanel.SetActive(true);
            if (txtResultScore != null)
            {
                txtResultScore.text = $"TOTAL SCORE: {finalScore:N0}";
            }
        }
    }
}