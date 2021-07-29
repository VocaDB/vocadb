import Layout from '@Components/Shared/Layout';
import HttpClient from '@Shared/HttpClient';
import StatsStore from '@Stores/StatsStore';
import Highcharts from 'highcharts';
import HighchartsReact from 'highcharts-react-official';
import { runInAction } from 'mobx';
import { observer } from 'mobx-react-lite';
import React from 'react';

const httpClient = new HttpClient();

const statsStore = new StatsStore(httpClient);

const StatsIndex = observer(
	(): React.ReactElement => {
		return (
			<Layout title="Statistics / Reports" /* TODO: localize */>
				<select
					value={JSON.stringify(statsStore.selectedReport)}
					onChange={(e): void =>
						runInAction(() => {
							statsStore.selectedReport = JSON.parse(e.target.value);
						})
					}
					className="input-large"
				>
					{statsStore.categories.map((category) => (
						<optgroup label={category.name} key={category.name}>
							{category.reports.map((report) => (
								<option value={JSON.stringify(report)} key={report.name}>
									{report.name}
								</option>
							))}
						</optgroup>
					))}
				</select>{' '}
				{statsStore.showTimespanFilter && (
					<select
						value={statsStore.timespan}
						onChange={(e): void =>
							runInAction(() => {
								statsStore.timespan = e.target.value;
							})
						}
					>
						<option value="">Overall{/* TODO: localize */}</option>
						<option value="24">Last day{/* TODO: localize */}</option>
						<option value="48">Last two days{/* TODO: localize */}</option>
						<option value="168">Last week{/* TODO: localize */}</option>
						<option value="720">Last month{/* TODO: localize */}</option>
						<option value="8760">Last year{/* TODO: localize */}</option>
						<option value="17520">Last 2 years{/* TODO: localize */}</option>
						<option value="26280">Last 3 years{/* TODO: localize */}</option>
						<option value="43800">Last 5 years{/* TODO: localize */}</option>
					</select>
				)}
				{statsStore.chartData && (
					<HighchartsReact
						highcharts={Highcharts}
						options={statsStore.chartData}
						immutable={true}
					/>
				)}
			</Layout>
		);
	},
);

export default StatsIndex;
