function createTask() {
  document.getElementById('createTaskForm').addEventListener('submit', function (event) {
    event.preventDefault();

    const taskDescription = document.getElementById('taskDescription').value;

    fetch('/Task/CreateTask', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ TaskDescription: taskDescription })
    })
      .then(response => {
        if (response.redirected) {
          window.location.href = response.url;
        } else {
          return response.json();
        }
      })
      .then(data => {
        console.log(data);
      })
      .catch(error => console.error('Error:', error));
  });
}