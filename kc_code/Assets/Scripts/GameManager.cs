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
    public GameObject accidentPanel;

    [Header("--- 효과 및 연출 ---")]
    public Image fadeImage;
    public TextMeshProUGUI tipText;

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

    [Header("--- 퀴즈 시스템 ---")]
    public GameObject quizTryPanel;       // 퀴즈 풀지 말지 여부 확인 패널
    public GameObject quizPanel;          // 퀴즈 팝업 패널
    public TextMeshProUGUI quizQuestion;  // 문제 텍스트
    public Button[] quizButtons;          // 보기 버튼 3개
    public TextMeshProUGUI[] quizBtnTexts;// 보기 버튼의 텍스트 3개
    public GameObject quizCorrectPanel;   // 정답일 때 띄울 패널
    public GameObject quizWrongPanel;     // 틀렸을 때 띄울 패널
    public TextMeshProUGUI txtWrongAnswer; // "정답은 2번입니다!" 라고 써줄 텍스트

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
    public int maxJobLevel = 10;
    public int salaryIncreasePerLevel = 200000;

    // [추가] 행동 비용
    public int studyCost = 150000;
    public int promotionCost = 1000000;
    public int convenienceCost = 100000;

    private bool isSavingsJoined = false;
    private bool hasSelfDevThisMonth = false;
    private bool hasQuizThisMonth = false; // [추가] 이번 달 퀴즈 수행 여부

    // [수정] 게임 내 콘텐츠와 직접 관련된 팁으로 교체
    private string[] financialFacts = new string[]
    {
        "[Tip!] 적금에 가입하면 만기 시 '원금 + 이자'를\n받을 수 있어 목돈 마련에 유리합니다.",
        "[Tip!] '복리'란 이자에 또 이자가 붙는 효과입니다.\n게임에서도 저축액이 늘어날수록 이자가 커집니다!",
        "[Tip!] 스트레스가 100%가 되면 건강을 잃고 게임이 종료됩니다.\n적절한 휴식도 투자입니다.",
        "[Tip!] 갑작스러운 교통사고에 대비해 항상\n'비상금(현금)'을 남겨둬야 파산을 막을 수 있습니다.",
        "[Tip!] 대출을 받으면 매달 이자가 지출됩니다.\n감당할 수 있는 능력 안에서만 빌리세요.",
        "[Tip!] 월급 명세서의 '실수령액'은 세금과 보험료를 뗀,\n실제로 내가 쓸 수 있는 돈입니다.",
        "[Tip!] 현금이 바닥나면 파산하게 됩니다.\n수입보다 지출이 많지 않도록 관리하세요."
    };

    [System.Serializable]
    public struct QuizData
    {
        public string question;
        public string[] answers;
        public int correctAnswer; // 0, 1, 2 중 하나
    }

    private QuizData[] quizzes = new QuizData[]
    {
        new QuizData { question = "'복리'의 의미로 올바른 것은?", answers = new string[] { "원금에만 이자가 붙음", "이자에 이자가 붙음", "대출 이자가 줄어듦" }, correctAnswer = 1 },
        new QuizData { question = "예금자보호법으로 보호받는 한도는?", answers = new string[] { "인당 3천만원", "인당 5천만원", "인당 1억원" }, correctAnswer = 1 },
        new QuizData { question = "주식에서 기업이 이익을 주주에게 나눠주는 것은?", answers = new string[] { "배당", "이자", "상환" }, correctAnswer = 0 },
        new QuizData { question = "물가가 지속적으로 오르는 현상은?", answers = new string[] { "디플레이션", "스태그플레이션", "인플레이션" }, correctAnswer = 2 },
        new QuizData { question = "신용점수가 낮아지면 발생하는 불이익은?", answers = new string[] { "대출 금리 상승", "취업 즉시 제한", "은행 이용 불가" }, correctAnswer = 0 },
        new QuizData { question = "분산 투자의 중요성을 강조한 격언은?", answers = new string[] { "계란을 한 바구니에 담지 마라", "티끌 모아 태산", "소 잃고 외양간 고친다" }, correctAnswer = 0 },
        new QuizData { question = "소득에서 세금 등을 뺀 실제 쓸 수 있는 돈은?", answers = new string[] { "총급여", "실수령액", "기본급" }, correctAnswer = 1 },
        new QuizData { question = "은행에 돈을 맡기는 가장 안전한 방법은?", answers = new string[] { "주식", "예금", "가상화폐" }, correctAnswer = 1 },
        new QuizData { question = "국가에 납부하는 필수 비용은?", answers = new string[] { "기부금", "세금", "배당금" }, correctAnswer = 1 },
        new QuizData { question = "수입보다 지출이 많을 때 발생하는 상태는?", answers = new string[] { "흑자", "적자", "무역" }, correctAnswer = 1 },
        new QuizData { question = "돈의 가치가 떨어지고 물가가 오르는 이유는?", answers = new string[] { "화폐 공급 증가", "화폐 공급 감소", "수입 증가" }, correctAnswer = 0 },
        new QuizData { question = "자산을 현금으로 바꿀 수 있는 정도는?", answers = new string[] { "수익성", "안정성", "유동성" }, correctAnswer = 2 },
        new QuizData { question = "기업의 소유권을 나타내는 유가증권은?", answers = new string[] { "주식", "채권", "보험" }, correctAnswer = 0 },
        new QuizData { question = "중앙은행이 결정하는 기본 금리는?", answers = new string[] { "시장금리", "우대금리", "기준금리" }, correctAnswer = 2 },
        new QuizData { question = "돈을 빌려준 대가로 받는 돈은?", answers = new string[] { "이자", "원금", "할부금" }, correctAnswer = 0 },
        new QuizData { question = "미래의 위험에 대비해 돈을 내는 제도는?", answers = new string[] { "적금", "주식", "보험" }, correctAnswer = 2 },
        new QuizData { question = "지출을 줄이기 위해 가장 먼저 할 일은?", answers = new string[] { "가계부 작성", "카드 추가 발급", "대출 받기" }, correctAnswer = 0 },
        new QuizData { question = "가격이 오를 것으로 예상해 미리 사는 행위는?", answers = new string[] { "충동구매", "사재기", "과소비" }, correctAnswer = 1 },
        new QuizData { question = "나라 간의 돈을 바꾸는 비율은?", answers = new string[] { "금리", "환율", "주가" }, correctAnswer = 1 },
        new QuizData { question = "일정 기간 돈을 맡기기로 약속하는 저축은?", answers = new string[] { "보통예금", "정기예금", "자유적금" }, correctAnswer = 1 },
        new QuizData { question = "개인의 경제적 신용도를 숫자로 나타낸 것은?", answers = new string[] { "신용점수", "시험점수", "통장잔고" }, correctAnswer = 0 },
        new QuizData { question = "체크카드와 신용카드의 가장 큰 차이점은?", answers = new string[] { "결제 방식", "카드 색상", "사용 가능 연령" }, correctAnswer = 0 },
        new QuizData { question = "경기가 안 좋을 때 정부가 주로 하는 일은?", answers = new string[] { "금리 인상", "금리 인하", "세금 인상" }, correctAnswer = 1 },
        new QuizData { question = "원금 손실 가능성이 있는 금융 상품은?", answers = new string[] { "정기예금", "펀드/주식", "청약저축" }, correctAnswer = 1 },
        new QuizData { question = "월급에서 미리 떼어가는 세금 제도는?", answers = new string[] { "원천징수", "연말정산", "부가세" }, correctAnswer = 0 },
        new QuizData { question = "상품의 가치를 나타내는 척도는?", answers = new string[] { "무게", "크기", "가격" }, correctAnswer = 2 },
        new QuizData { question = "한 나라의 경제 규모를 나타내는 지표는?", answers = new string[] { "GDP", "CPI", "KOSPI" }, correctAnswer = 0 },
        new QuizData { question = "빚을 갚지 못해 법적으로 선언하는 상태는?", answers = new string[] { "파산", "정지", "해지" }, correctAnswer = 0 },
        new QuizData { question = "비상금을 마련해야 하는 가장 큰 이유는?", answers = new string[] { "쇼핑하려고", "갑작스러운 지출 대비", "이자 받으려고" }, correctAnswer = 1 },
        new QuizData { question = "금융 거래 시 본인임을 확인하는 수단은?", answers = new string[] { "공동인증서", "학생증", "멤버십카드" }, correctAnswer = 0 }
    };

    void Awake() { instance = this; }

    void Start()
    {
        CloseAllPanels();
        UpdateUI();

        if (tipText) tipText.gameObject.SetActive(false);

        if (!isGameStarted)
        {
            Time.timeScale = 0;
            if (startPanel) startPanel.SetActive(true);

            // [수정] 타이틀 BGM 재생
            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayBGM(SoundManager.instance.titleBgm);
            }
        }
        else
        {
            Time.timeScale = 1;
            if (startPanel) startPanel.SetActive(false);

            if (SoundManager.instance) SoundManager.instance.PlayHomeBGM();
        }
    }

    public void GameStart()
    {
        isGameStarted = true;
        isGameOver = false;
        Time.timeScale = 1;
        if (startPanel) startPanel.SetActive(false);
        UpdateUI();

        if (SoundManager.instance)
            SoundManager.instance.PlayHomeBGM();
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
        hasQuizThisMonth = false; // [추가]
    }

    public void OnClickNextMonth()
    {
        if (currentMonth > 10 || isGameOver) return;

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
        stress += 40;

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
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        if (reportPanel) reportPanel.SetActive(false);
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        if (tipText != null && financialFacts.Length > 0)
        {
            tipText.gameObject.SetActive(true);
            int randIdx = Random.Range(0, financialFacts.Length);
            tipText.text = financialFacts[randIdx];
        }

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float t = 0;
            // 어두워지기 (Fade In)
            while (t < 1)
            {
                t += Time.unscaledDeltaTime * 2f;
                fadeImage.color = new Color(0, 0, 0, t);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, 1);
        }

        // [수정] 1초만 대기 후 진행 (빠른 템포)
        yield return new WaitForSecondsRealtime(1.0f);

        currentMonth++;
        hasSelfDevThisMonth = false;
        hasQuizThisMonth = false; // [추가] 매달 퀴즈 기회 리셋

        if (currentMonth > 10)
        {
            if (tipText) tipText.gameObject.SetActive(false);
            EndGame("완료");
            yield break;
        }

        if (stress >= 100)
        {
            if (tipText) tipText.gameObject.SetActive(false);
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

        if (tipText) tipText.gameObject.SetActive(false);

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

        // 추가된 패널들 끄기
        if (quizTryPanel) quizTryPanel.SetActive(false);
        if (quizPanel) quizPanel.SetActive(false);
        if (quizCorrectPanel) quizCorrectPanel.SetActive(true); // 활성화 되어있다면 끄기 위해 수정
        if (quizWrongPanel) quizWrongPanel.SetActive(true); // 활성화 되어있다면 끄기 위해 수정
        
        // 실제로 끄기
        if (quizCorrectPanel) quizCorrectPanel.SetActive(false);
        if (quizWrongPanel) quizWrongPanel.SetActive(false);

        // 중요 패널들이 다 꺼졌다면 시간 재개
        if (!reportPanel.activeSelf && !accidentPanel.activeSelf && !quizPanel.activeSelf && !quizTryPanel.activeSelf)
        {
            Time.timeScale = 1;
        }
    }

    public void ActionPromotion()
    {
        if (isGameOver) return;
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
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);
        if (isGameOver || loan >= 2000000) return; loan += 500000; cash += 500000; UpdateUI();
    }

    public void ActionRepay()
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);
        if (isGameOver || loan <= 0) return; if (cash < 500000) { ShowInsufficientFundsPanel(); return; }
        loan -= 500000; cash -= 500000; UpdateUI();
    }

    public void ActionJoinSavings()
    {
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

        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.eatSfx);

        if (alertPanel)
        {
            alertPanel.SetActive(true);
            if (txtAlertMsg) txtAlertMsg.text = "편의점 이용 완료!\n스트레스 -30";
        }
    }

    public void ActionRest()
    {
        if (isGameOver) return;
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

        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.crashSfx);

        if (accidentPanel)
        {
            accidentPanel.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void OnConfirmAccident()
    {
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

    // --- [추가/수정] 1단계: 퀴즈 참여 여부 확인 패널 띄우기 ---
    public void ShowQuizConfirm()
    {
        if (quizTryPanel == null || isGameOver) return;

        // 이번 달 이미 참여했는지 체크
        if (hasQuizThisMonth)
        {
            if (alertPanel)
            {
                alertPanel.SetActive(true);
                if (txtAlertMsg) txtAlertMsg.text = "퀴즈는 한 달에 한 번만 참여할 수 있습니다.";
            }
            return;
        }

        Time.timeScale = 0; // 게임 일시정지
        quizTryPanel.SetActive(true);
    }

    // --- [수정] 2단계: 실제 퀴즈 문제 패널 시작 ("예" 버튼 클릭 시) ---
    public void StartQuiz()
    {
        if (quizTryPanel) quizTryPanel.SetActive(false); // 확인창 닫기
        if (quizPanel == null) return;
        
        quizPanel.SetActive(true);

        int rand = Random.Range(0, quizzes.Length);
        QuizData selectedQuiz = quizzes[rand];
        quizQuestion.text = selectedQuiz.question;

        // 정답 번호 (1, 2, 3번 중 하나)
        int displayCorrectNum = selectedQuiz.correctAnswer + 1;

        for (int i = 0; i < 3; i++)
        {
            int index = i; 
            quizBtnTexts[i].text = selectedQuiz.answers[i];
            quizButtons[i].onClick.RemoveAllListeners();
            // OnClickAnswer 호출 시 정답 번호(displayCorrectNum) 전달
            quizButtons[i].onClick.AddListener(() => OnClickAnswer(index == selectedQuiz.correctAnswer, displayCorrectNum));
        }
    }

    // --- [수정] 답 선택 시 결과 패널 처리 ---
    void OnClickAnswer(bool isCorrect, int correctNum)
    {
        if (SoundManager.instance) SoundManager.instance.PlaySFX(SoundManager.instance.clickSfx);

        hasQuizThisMonth = true;
        quizPanel.SetActive(false); 

        if (isCorrect)
        {
            cash += 100000;
            UpdateUI();
            if (quizCorrectPanel) quizCorrectPanel.SetActive(true); 
        }
        else
        {
            if (quizWrongPanel)
            {
                quizWrongPanel.SetActive(true);
                if (txtWrongAnswer != null)
                    txtWrongAnswer.text = $"오답입니다!\n정답은 <color=yellow>{correctNum}번</color>입니다.";
            }
        }
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
        if (quizTryPanel) quizTryPanel.SetActive(false);
        if (quizPanel) quizPanel.SetActive(false);
        if (quizCorrectPanel) quizCorrectPanel.SetActive(false);
        if (quizWrongPanel) quizWrongPanel.SetActive(false);
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
        if (tipText != null) tipText.gameObject.SetActive(false);

        CloseAllPanels();

        if (SoundManager.instance)
        {
            SoundManager.instance.StopBGM();

            if (type == "스트레스") SoundManager.instance.PlaySFX(SoundManager.instance.failStressBgm);
            else if (type == "파산") SoundManager.instance.PlaySFX(SoundManager.instance.failMoneyBgm);
            else SoundManager.instance.PlaySFX(SoundManager.instance.successBgm);
        }

        if (type == "스트레스")
        {
            failStressPanel.SetActive(true);
        }
        else if (type == "파산")
        {
            UpdateUI(); failMoneyPanel.SetActive(true);
        }
        else
        {
            successPanel.SetActive(true);
            if (txtResultScore != null) txtResultScore.text = $" {(cash + savings - loan):N0}";
        }
    }
}