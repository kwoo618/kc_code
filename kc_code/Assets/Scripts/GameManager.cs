using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("--- UI 텍스트 연결 (TMP) ---")]
    public TextMeshProUGUI monthText;      // 상단: 1개월차
    public TextMeshProUGUI cashText;       // 상단: 현금
    public TextMeshProUGUI savingsText;    // 상단: 적금
    public TextMeshProUGUI loanText;       // 상단: 빚
    public TextMeshProUGUI stressText;     // 상단: 스트레스
    public TextMeshProUGUI salaryText;     // 상단: 월급 정보

    [Header("--- 팝업 패널 연결 ---")]
    public GameObject bankPanel;           // 은행 창
    public GameObject storePanel;          // 상점 창
    public GameObject academyPanel;        // 학원 창
    public GameObject reportPanel;         // 월급 명세서 창
    public TextMeshProUGUI reportContent;  // 명세서 내용
    public GameObject resultPanel;         // 엔딩 창
    public TextMeshProUGUI resultTitle;    // 엔딩 제목
    public TextMeshProUGUI resultDesc;     // 엔딩 내용
    public GameObject msgPanel;            // 알림 토스트 창
    public TextMeshProUGUI msgText;        // 알림 내용

    [Header("--- 버튼 상태 제어 ---")]
    public Button savingsJoinBtn;          // 적금 가입 버튼 (가입 후 비활성화용)
    public TextMeshProUGUI savingsBtnText;

    [Header("--- 게임 데이터 ---")]
    public int currentMonth = 1;
    public long cash = 300000;
    public long savings = 0;
    public long loan = 0;
    public int stress = 0;
    public int jobLevel = 1;

    // 밸런스 상수
    private const int BASE_SALARY = 2000000;
    private const int MONTHLY_SAVINGS_AMOUNT = 500000;
    private const int LIVING_COST = 500000;
    private bool isSavingsJoined = false;

    void Awake() { instance = this; }
    void Start() { UpdateUI(); CloseAllPanels(); }

    // [다음 달] 버튼 연결 함수
    public void OnClickNextMonth()
    {
        if (currentMonth >= 10) { EndGame("졸업"); return; }

        int currentSalary = BASE_SALARY + ((jobLevel - 1) * 100000);

        // 1. 공제 계산 (국민연금, 건강보험, 소득세)
        int pension = (int)(currentSalary * 0.045f);
        int health = (int)(currentSalary * 0.035f);
        int tax = (int)(currentSalary * 0.03f);
        int totalDeduction = pension + health + tax;
        int netPay = currentSalary - totalDeduction;

        // 2. 대출 이자 (2%)
        int loanInterest = 0;
        if (loan > 0)
        {
            loanInterest = (int)(loan * 0.02f);
            netPay -= loanInterest;
        }

        // 3. 적금 자동이체
        string savingsMsg = "";
        if (isSavingsJoined)
        {
            if (cash + netPay >= MONTHLY_SAVINGS_AMOUNT)
            {
                savings += MONTHLY_SAVINGS_AMOUNT;
                netPay -= MONTHLY_SAVINGS_AMOUNT;
                savingsMsg = $"<color=blue>적금 자동이체: -{MONTHLY_SAVINGS_AMOUNT:N0}</color>\n";
            }
            else savingsMsg = $"<color=red>* 잔액 부족 이체 실패</color>\n";
        }

        // 4. 적금 이자 수익 (0.5%)
        int interest = (int)(savings * 0.005f);
        savings += interest;

        // 5. 최종 반영
        cash += netPay;
        cash -= LIVING_COST;

        // 6. 스트레스 증가
        stress += 20;
        if (stress > 100) stress = 100;

        // 명세서 텍스트 작성
        string report = $"<size=120%><b>{currentMonth}월 급여 명세서</b></size>\n\n" +
                        $"세전 월급: {currentSalary:N0}\n" +
                        $"--------------------\n" +
                        $"<color=red>국민연금(4.5%): -{pension:N0}</color>\n" +
                        $"<color=red>건강보험(3.5%): -{health:N0}</color>\n" +
                        $"<color=red>소득세 등: -{tax:N0}</color>\n" +
                        (loan > 0 ? $"<color=red>대출 이자(2%): -{loanInterest:N0}</color>\n" : "") +
                        $"--------------------\n" +
                        $"<b>실수령액: {(currentSalary - totalDeduction - loanInterest):N0}</b>\n\n" +
                        $"<color=red>고정 생활비: -{LIVING_COST:N0}</color>\n" +
                        savingsMsg +
                        $"<color=blue>적금 이자 수익: +{interest:N0}</color>\n" +
                        $"--------------------\n" +
                        $"<b>최종 현금 변동: {(netPay - LIVING_COST):N0}</b>";

        reportContent.text = report;
        reportPanel.SetActive(true);

        currentMonth++;

        if (stress >= 100) EndGame("스트레스");
        else if (cash < 0) EndGame("파산");

        UpdateUI();
    }

    // --- 행동 함수 (버튼 연결용) ---
    public void ActionBorrow() // 대출 받기
    {
        if (loan >= 2000000) { ShowToast("한도 초과!"); return; }
        loan += 500000; cash += 500000;
        UpdateUI(); ShowToast("50만원 대출 완료"); bankPanel.SetActive(false);
    }

    public void ActionRepay() // 대출 상환
    {
        if (loan <= 0) { ShowToast("갚을 빚이 없습니다."); return; }
        if (cash < 500000) { ShowToast("현금이 부족합니다."); return; }
        loan -= 500000; cash -= 500000;
        UpdateUI(); ShowToast("50만원 상환 완료"); bankPanel.SetActive(false);
    }

    public void ActionJoinSavings() // 적금 가입
    {
        if (isSavingsJoined) return;
        isSavingsJoined = true;
        UpdateUI(); ShowToast("적금 가입 완료!"); bankPanel.SetActive(false);
    }

    public void ActionBuy() // 소비 (상점)
    {
        if (cash < 50000) { ShowToast("돈이 부족합니다."); return; }
        cash -= 50000; stress -= 30; if (stress < 0) stress = 0;
        UpdateUI(); ShowToast("쇼핑 완료!"); storePanel.SetActive(false);
    }

    public void ActionStudy() // 공부 (학원)
    {
        if (cash < 100000) { ShowToast("수강료 부족!"); return; }
        cash -= 100000; jobLevel++; stress += 10; if (stress > 100) stress = 100;
        UpdateUI(); ShowToast("직무 레벨 상승!"); academyPanel.SetActive(false);
    }

    public void ActionRest() // 휴식 (학원/집)
    {
        stress -= 10; if (stress < 0) stress = 0;
        UpdateUI(); ShowToast("휴식 완료"); academyPanel.SetActive(false);
    }

    public void CloseAllPanels()
    {
        if (bankPanel) bankPanel.SetActive(false);
        if (storePanel) storePanel.SetActive(false);
        if (academyPanel) academyPanel.SetActive(false);
        if (reportPanel) reportPanel.SetActive(false);
        if (resultPanel) resultPanel.SetActive(false);
        if (msgPanel) msgPanel.SetActive(false);
    }

    void UpdateUI()
    {
        monthText.text = $"{currentMonth}개월차";
        cashText.text = $"{cash:N0}";
        savingsText.text = $"{savings:N0}";
        loanText.text = $"{loan:N0}";
        stressText.text = $"{stress}%";
        stressText.color = stress > 80 ? Color.red : Color.white;
        int curSal = BASE_SALARY + ((jobLevel - 1) * 100000);
        salaryText.text = $"{curSal:N0}";

        if (isSavingsJoined && savingsJoinBtn != null)
        {
            savingsJoinBtn.interactable = false;
            if (savingsBtnText != null) savingsBtnText.text = "가입 완료";
        }
    }

    void ShowToast(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(ToastProcess(msg));
    }

    IEnumerator ToastProcess(string msg)
    {
        msgPanel.SetActive(true);
        msgText.text = msg;
        yield return new WaitForSeconds(2f);
        msgPanel.SetActive(false);
    }

    void EndGame(string type)
    {
        resultPanel.SetActive(true);
        long total = cash + savings - loan;
        string title = "", desc = "";

        if (type == "스트레스") { title = "게임 오버"; desc = "스트레스 과다로 쓰러졌습니다."; }
        else if (type == "파산") { title = "파산"; desc = "빚을 갚지 못했습니다."; }
        else
        {
            title = "졸업 축하";
            desc = $"최종 현금: {cash:N0}\n최종 적금: {savings:N0}\n남은 대출: -{loan:N0}\n\n<b>순자산: {total:N0}</b>";
        }
        resultTitle.text = title;
        resultDesc.text = desc;
    }
}