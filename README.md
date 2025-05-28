# HeyCheese

<br/>

## 깃허브 커밋 규칙
- Ref: [Git Commit Message Convention](https://github.com/gyoogle/tech-interview-for-developer/blob/master/ETC/Git%20Commit%20Message%20Convention.md)

**커밋 메세지 형식**
> type: Subject (제목)
> <br/>
> body (본문)
> <br/>
> footer (꼬리말)

- `feat` : 새로운 기능에 대한 커밋
- `fix` : 버그 수정에 대한 커밋
- `build` : 빌드 관련 파일 수정에 대한 커밋
- `chore` : 그 외 자잘한 수정에 대한 커밋
- `ci` : CI관련 설정 수정에 대한 커밋
- `docs` : 문서 수정에 대한 커밋
- `style` : 코드 스타일 혹은 포맷 등에 관한 커밋
- `refactor` : 코드 리팩토링에 대한 커밋
- `test` : 테스트 코드 수정에 대한 커밋

**Subject (제목)**

- *한글*로 간결하게 작성

**Body (본문)**

- 상세히 작성, 기본적으로 무엇을 왜 진행 하였는지 작성
- Issue 등록 시, Issue 태그

**footer (꼬리말)**

- 참고사항

<br/>

## 깃허브 브랜치 규칙
- Ref: [Git Branch & Naming](https://ej-developer.tistory.com/75)

**크게 3가지 유형의 브랜치로 분기하여 사용**

- `main` : 유저에게 배포가능한 상태를 관리하는 브랜치. 절대 함부로 병합 시키지 말것
- `develop` : 기능개발을 위한 브랜들을 병합시키는 브랜치. feature/... 브랜치는 이곳에서 분기하여 병합, 안정적인 상태일때, main에 병합
- `feature/...` : 새로운 기능 및 버그 수정이 필요할 때 사용하는 브랜치. develop 브랜치에서 분기하여 병합, 더 이상 필요가 없다면 삭제 naming ex) feature/dialogue ex) main -> develop -> feature 분기 feature -> develop -> main 병합

## PR 규칙
**PR 제목**: 03.26 작업 제목

개인 브랜치 -> merge 브랜치에 pr
문제 없으면 merge 브랜치 -> main 에 머지

**Comment**

- 작업에 대한 자세한 내용(이유)
- 작업 내용
- 미리보기(첨부)
<img width="640" alt="img1 daumcdn" src="https://github.com/user-attachments/assets/feda2e1e-8965-46fc-8548-48fc970402c8" />

## 메인 가져오기
### 기존 변경사항 잠시 버려두기 (해당 하지 않으면 '메인 가져오기'로 ->)
⚠️가능하면 Changes가 없을 때 가져오는 것을 추천 (충돌이 날 수 있기 때문..)
간혹 변경 사항을 없앤 후 받아오라는 문구가 뜰 때가 있는데
![image.png](attachment:1a865f13-66e7-4113-945b-208644dd4f1e:image.png)
- 11 changed files 부분 우클릭 후 Stash all changes (잠시 버리기)
⚠️Discard all changes는 영구 삭제이므로 유의할 것!
- 이 상태에서 메인 가져오기(1번~마지막까지 진행)
![image.png](attachment:0df696a4-bf63-4199-a97b-356c1f1c024e:image.png)
- 이후 View Stash를 누르면 임의로 버린 파일들을 볼 수 있음
- ![image.png](attachment:5b52f818-c44e-40b6-9664-e05a1a672a93:image.png)
- Restore 클릭 시 임시로 버렸던 변경사항들이 다시 돌아옴
⚠️Discard는 영구 삭제이므로 유의할 것!

### 메인 가져오기
![image.png](attachment:bcb9f74a-e679-4561-85f2-510fad41273e:image.png)
![image.png](attachment:45dc5713-9cdb-40c2-8717-915a75451624:image.png)
![image.png](attachment:4b08b449-e11f-4098-9d52-ce3718870123:image.png)
- main 선택 후 ‘Creat a merge commit’ 클릭
![image.png](attachment:fd3e8e51-97ab-4428-baf1-51ed8824d1a1:image.png)
![image.png](attachment:19c096dd-a49b-4bc0-9334-f47acdb67989:image.png)
