//niente classe
//linguaggio procedurale

import { ArticleService } from '../service/ArticleService.js';
import { ArticleFirebaseRepository } from '../repository/ArticleFirebaseRepository.js';


const articleService = new ArticleService(new ArticleFirebaseRepository());

const btnRefresh = document.getElementById('btnRefresh');
const btnSave = document.getElementById('btnSave');
const btnDelete = document.getElementById('btnDelete');
const btnClear = document.getElementById('btnClear');
const btnUpdate = document.getElementById('btnUpdate');
const listEl = document.getElementById('postLists');

function renderArticleDetail(article) {
  document.getElementById('inputTitle').value = article.Title || 'Senza titolo';
  document.getElementById('inputContent').value = article.Content || '';
}

async function init() {
  try {
    listEl.innerHTML = '';
    const articles = await articleService.getAll();
    articles.forEach((article, index) => {
      const btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'list-group-item list-group-item-action text-start py-2 px-2';
      btn.dataset.id = article.Id;
      btn.innerHTML = `
        <div class="fw-medium small">${article.Title || 'Senza titolo'}</div>
        <div class="article-date mt-1">${article.formattedDate()}</div>
      `;
      btn.addEventListener('click', () => {
        listEl.querySelectorAll('.list-group-item').forEach(el => el.classList.remove('active'));
        btn.classList.add('active');
        renderArticleDetail(article);
      });
      listEl.appendChild(btn);
      if (index === 0) btn.click();
    });
  } catch (err) {
    console.error('Errore nel caricamento degli articoli:', err);
  }
}

init();

btnRefresh.addEventListener('click', async () => {
  listEl.innerHTML = '';
  await init();
});

btnClear.addEventListener('click', (e) => {
  e.preventDefault();
  document.getElementById('inputTitle').value ='';
  document.getElementById('inputContent').value = '';
});

btnSave.addEventListener('click', async (e) => {
  e.preventDefault();
  const newArticle = {
    Title: document.getElementById('inputTitle').value,
    Content: document.getElementById('inputContent').value
  };
  await articleService.save(newArticle);
  await init();
});

btnDelete.addEventListener('click', async (e) => {
  e.preventDefault();
  const articleToDelete = listEl.querySelector(".list-group-item.active");
  await articleService.delete(articleToDelete.dataset.id);
  await init();
});

btnUpdate.addEventListener('click', async (e) => {
  e.preventDefault();
  const idToUpdate = listEl.querySelector(".list-group-item.active").dataset.id;

  const newArticle = {
    Title: document.getElementById('inputTitle').value,
    Content: document.getElementById('inputContent').value
  };

  await articleService.update(idToUpdate,newArticle);
});