using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private static bool isGameStarted = false;

    public bool isGameOver = false;

    [Header("--- 플레이어 설정 ---")]
    public GameObject player;

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

    [Header("--- 알림용 패널 ---")]
    public GameObject alertPanel;
    public GameObject insufficientFundsPanel;
    public GameObject duplicateActionPanel;
    public GameObject highStressPanel;
    public GameObject lowStressPanel;
    public GameObject accidentPanel;    // 교통사고 전용 패널

    [Header("--- 효과 및 연출 ---")]
    public Image fadeImage;

    [Header("--- 상단 HUD ---")]
    public TextMeshProUGUI monthText;
    public TextMeshProUGUI cashText;
    public TextMeshProUGUI savingsText;
    public TextMeshProUGUI loanText;
    public TextMeshProUGUI stressText;
    public TextMeshProUGUI salaryText;
    public TextMeshProUGUI jobLevelText;

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

    [Header("--- 게임 데이터 및 밸런스 ---")]
    public int currentMonth = 1;
    public long cash = 300000;
    public long savings = 0;
    public long loan = 0;
    public int stress = 0;
    public int jobLevel = 1;
    public int baseSalary = 2000000;

    // [밸런스] 수치 조정 영역
    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 1000000;

    // [추가] 밸런스 변수
    public int maxJobLevel = 10;                // 최대 레벨 제한
    public int salaryIncreasePerLevel = 200000; // 레벨당 월급 인상액

    // [추가] 행동 비용
    public int studyCost = 150000;              // 강의 비용
    public int promotionCost = 1000000;         // 실무 비용
    public int convenienceCost = 100000;         // 편의점 비용

    private bool isSavingsJoined = false;
    private bool hasSelfDevThisMonth = false;

    void Awake() { instance = this; }

    void Start()
    {
        CloseAllPanels();
        UpdateUI();

        if (!isGameStarted)
        {
            Time.timeScale = 0;
            if (startPanel) startPanel.SetActive(true);

            // [Sound] 타이틀 화면 배경음
            if (SoundManager.instance) SoundManager.instance.PlayBGM(SoundManager.instance.titleBgm);
        }
        else
        {
            Time.timeScale = 1;
            if (startPanel) startPanel.SetActive(false);

            // [Sound] 게임 중 거리 배경음
            if (SoundManager.instance) SoundManager.instance.PlayMainBGM();
        }
    }

    public void GameStart()
    {
        isGameStarted = true;
        isGameOver = false;
        Time.timeScale = 1;
        if (startPanel) startPanel.SetActive(false);
        UpdateUI();

        // [Sound] 게임 시작 -> 거리 배경음 재생
        if (SoundManager.instance) SoundManager.instance.PlayMainBGM();
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
        SceneManager.LoadScene("CompanyMain");
    }

    public void GameRetry()
    {
        isGameStarted = true;
        ResetGameData();
        SceneManager.LoadScene("CompanyMain");
    }

    void ResetGameData()
    {
        Time.timeScale = 1;
        isGameOver = false;
        currentMonth = 1;
        cash = 300000;
        stress = 0;
        savings = 0;
        loan = 0;
        jobLevel = 1;
        baseSalary = 2000000;
        isSavingsJoined = false;
        hasSelfDevThisMonth = false;
    }

    public void OnClickNextMonth()
    {
        if (currentMonth > 10 || isGameOver) return;

        // [Sound] 버튼 클릭음 (다음달 넘기기)
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.nextMonthSfx);

        if (stress + 40 >= 100)
        {
            ShowHighStressPanel();
            return;
        }

        if (cash < 0)
        {
            EndGame("파산");
            return;
        }

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

        if (loan > 0) netPay -= (int)(loan * 0.02f);

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
        stress += 40; // [밸런스]

        UpdateUI();

        if (txtReportSalary) txtReportSalary.text = $"{currentSalary:N0}";
        if (txtReportPension) txtReportPension.text = $"-{pension:N0}";
        if (txtReportHealth) txtReportHealth.text = $"-{health:N0}";
        if (txtReportTax) txtReportTax.text = $"-{tax:N0}";
        if (txtReportLoan) txtReportLoan.text = (loan > 0) ? $"-{(int)(loan * 0.02f):N0}" : "0";
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
        // [Sound] 확인 버튼 클릭음 (일반 클릭음)
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

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

        currentMonth++;
        hasSelfDevThisMonth = false;

        if (currentMonth > 10)
        {
            EndGame("완료");
            yield break;
        }

        if (stress >= 100)
        {
            EndGame("스트레스");
            yield break;
        }

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

    void ShowInsufficientFundsPanel() { CloseAllPanels(); if (insufficientFundsPanel) insufficientFundsPanel.SetActive(true); }
    void ShowHighStressPanel() { CloseAllPanels(); if (highStressPanel) highStressPanel.SetActive(true); }
    void ShowLowStressPanel() { CloseAllPanels(); if (lowStressPanel) lowStressPanel.SetActive(true); }
    void ShowDuplicateActionPanel() { CloseAllPanels(); if (duplicateActionPanel) duplicateActionPanel.SetActive(true); }

    public void CloseAlert()
    {
        if (alertPanel) alertPanel.SetActive(false);
        if (insufficientFundsPanel) insufficientFundsPanel.SetActive(false);
        if (highStressPanel) highStressPanel.SetActive(false);
        if (lowStressPanel) lowStressPanel.SetActive(false);
        if (duplicateActionPanel) duplicateActionPanel.SetActive(false);
        if (accidentPanel) accidentPanel.SetActive(false);
    }

    public void ActionPromotion()
    {
        if (isGameOver) return;

        // [Sound] 버튼 클릭음
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (hasSelfDevThisMonth) { ShowDuplicateActionPanel(); return; }

        if (jobLevel >= maxJobLevel)
        {
            if (alertPanel)
            {
                alertPanel.SetActive(true);
                if (txtAlertMsg) txtAlertMsg.text = "이미 최고 직무 레벨입니다.";
            }
            return;
        }

        if (stress + 40 >= 100) { ShowHighStressPanel(); return; }
        if (cash < promotionCost) { ShowInsufficientFundsPanel(); return; }

        cash -= promotionCost;

        int levelGain = 3;
        if (jobLevel + levelGain > maxJobLevel) levelGain = maxJobLevel - jobLevel;

        jobLevel += levelGain;
        baseSalary += (salaryIncreasePerLevel * levelGain);
        stress += 40;

        hasSelfDevThisMonth = true;
        CloseAllPanels(); UpdateUI();

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = $"실무 참여 완료!\n직무 레벨 +{levelGain}\n월급이 대폭 인상되었습니다.";
        }
    }

    public void ActionStudy()
    {
        if (isGameOver) return;

        // [Sound] 버튼 클릭음
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (hasSelfDevThisMonth) { ShowDuplicateActionPanel(); return; }

        if (jobLevel >= maxJobLevel)
        {
            if (alertPanel)
            {
                alertPanel.SetActive(true);
                if (txtAlertMsg) txtAlertMsg.text = "이미 최고 직무 레벨입니다.";
            }
            return;
        }

        if (stress + 10 >= 100) { ShowHighStressPanel(); return; }
        if (cash < studyCost) { ShowInsufficientFundsPanel(); return; }

        cash -= studyCost;
        jobLevel += 1;
        baseSalary += salaryIncreasePerLevel;
        stress += 10;

        hasSelfDevThisMonth = true;
        CloseAllPanels(); UpdateUI();

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = "직무 강의 수강 완료!\n직무 레벨 +1";
        }
    }

    public void ActionBorrow()
    {
        // [Sound] 클릭음
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);
        if (isGameOver || loan >= 2000000) return; loan += 500000; cash += 500000; UpdateUI();
    }

    public void ActionRepay()
    {
        // [Sound] 클릭음
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);
        if (isGameOver || loan <= 0) return; if (cash < 500000) { ShowInsufficientFundsPanel(); return; }
        loan -= 500000; cash -= 500000; UpdateUI();
    }

    public void ActionJoinSavings()
    {
        // [Sound] 클릭음
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (isGameOver || isSavingsJoined) return;
        isSavingsJoined = true;
        UpdateUI();

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = "정기적금 가입 완료!\n매달 500,000원이 저축됩니다.\n(게임 종료시 이자포함 지급)";
        }
    }

    public void ActionBuy()
    {
        if (isGameOver) return;
        if (stress <= 0) { ShowLowStressPanel(); return; }

        if (cash < convenienceCost) { ShowInsufficientFundsPanel(); return; }

        cash -= convenienceCost;
        stress -= 30;
        if (stress < 0) stress = 0;

        CloseAllPanels();
        UpdateUI();

        // [Sound] 편의점 먹는 소리 (냠냠)
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.eatSfx);

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = "스트레스 -30";
        }
    }

    public void ActionRest()
    {
        if (isGameOver) return;

        // [Sound] 휴식 효과음 (그냥 클릭음 사용)
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (stress <= 0) { ShowLowStressPanel(); return; }
        stress -= 20; if (stress < 0) stress = 0; UpdateUI();
    }

    public void OnCarAccident()
    {
        if (isGameOver || (accidentPanel != null && accidentPanel.activeSelf)) return;

        int penalty = 200000;

        cash -= penalty;
        stress += 20; if (stress > 100) stress = 100;

        UpdateUI();

        // [Sound] 쾅! 교통사고 소리
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.crashSfx);

        if (accidentPanel)
        {
            accidentPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void OnConfirmAccident()
    {
        // [Sound] 확인 버튼 클릭음
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (accidentPanel) accidentPanel.SetActive(false);

        if (cash < 0)
        {
            EndGame("파산");
            return;
        }

        if (stress >= 100)
        {
            EndGame("스트레스");
            return;
        }

        StopAllCoroutines();
        StartCoroutine(CarAccidentSequence());
    }

    IEnumerator CarAccidentSequence()
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
        }

        if (player != null) player.transform.position = new Vector3(98.8f, -65.56f, 0);

        currentMonth++;
        hasSelfDevThisMonth = false;

        if (currentMonth > 10) { EndGame("완료"); yield break; }

        UpdateUI();
        yield return new WaitForSecondsRealtime(0.5f);

        if (fadeImage != null)
        {
            float t = 1;
            while (t > 0)
            {
                t -= Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.gameObject.SetActive(false);
        }
        Time.timeScale = 1;
    }

    public void CloseAllPanels()
    {
        if (bankPanel) bankPanel.SetActive(false);
        if (storePanel) storePanel.SetActive(false);
        if (academyPanel) academyPanel.SetActive(false);
        if (reportPanel) reportPanel.SetActive(false);
        if (nextPanel) nextPanel.SetActive(false);
        if (alertPanel) alertPanel.SetActive(false);
        if (insufficientFundsPanel) insufficientFundsPanel.SetActive(false);
        if (duplicateActionPanel) duplicateActionPanel.SetActive(false);
        if (highStressPanel) highStressPanel.SetActive(false);
        if (lowStressPanel) lowStressPanel.SetActive(false);
        if (accidentPanel) accidentPanel.SetActive(false);
    }

    void UpdateUI()
    {
        if (monthText) monthText.text = $"{currentMonth}개월차";
        if (cashText) cashText.text = $"{cash:N0}";
        if (savingsText) savingsText.text = $"{savings:N0}";
        if (loanText) loanText.text = $"{loan:N0}";
        if (stressText) { stressText.text = $"{stress}%"; }
        if (salaryText) salaryText.text = $"{baseSalary:N0}";
        if (jobLevelText) jobLevelText.text = $"Lv. {jobLevel}";

        if (isSavingsJoined)
        {
            if (savingsBtnText) savingsBtnText.text = "가입 완료";
            if (savingsJoinBtn) savingsJoinBtn.interactable = false;
        }
    }

    void EndGame(string type)
    {
        isGameOver = true;
        Time.timeScale = 0;
        if (fadeImage != null) { fadeImage.color = new Color(0, 0, 0, 0); fadeImage.gameObject.SetActive(false); }
        CloseAllPanels();

        // [Sound] 배경음 끄고 결과 효과음 재생
        if (SoundManager.instance)
        {
            SoundManager.instance.StopBGM();

            if (type == "스트레스")
            {
                failStressPanel.SetActive(true);
                SoundManager.instance.PlaySFX(SoundManager.instance.failStressBgm);
            }
            else if (type == "파산")
            {
                UpdateUI(); failMoneyPanel.SetActive(true);
                SoundManager.instance.PlaySFX(SoundManager.instance.failMoneyBgm);
            }
            else
            {
                successPanel.SetActive(true);
                SoundManager.instance.PlaySFX(SoundManager.instance.successBgm);
                if (txtResultScore != null) txtResultScore.text = $" {(cash + savings - loan):N0}";
            }
        }
    }
}