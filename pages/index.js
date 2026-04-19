//niente classe
//linguaggio procedurale

import { ArticleService } from '../service/ArticleService.js';
import { ArticleFirebaseRepository } from '../repository/ArticleFirebaseRepository.js';


const articleService = new ArticleService(new ArticleFirebaseRepository());

function renderArticleDetail(article) {
  document.getElementById('inputTitle').value = article.Title || 'Senza titolo';
  document.getElementById('inputContent').textContent = article.Content || '';
}

async function init() {
  const listEl = document.getElementById('postLists');
  const btnRefresh = document.getElementById('btnRefresh');

  btnRefresh.addEventListener('click', () => {
    init();
  })

  try {
    const articles = await articleService.getAll();
    articles.forEach((article, index) => {
      const btn = document.createElement('button');
      btn.type = 'button';
      btn.className = 'list-group-item list-group-item-action text-start py-2 px-2';
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